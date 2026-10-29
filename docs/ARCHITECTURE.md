# Phileas – Technical Design Document

Phileas is a WPF/MVVM desktop application that solves the travelling salesman problem (TSP) with two metaheuristics,
**Simulated Annealing** and **Tabu Search**, and lets you watch and compare them. It is the modern .NET rewrite of the
thesis application in this repository (`TezRota`, WinForms, .NET Framework 4.8).

This document follows the structure of the original review request: analysis → modernization → roadmap → architecture →
folders → UI/UX → visualization → MVVM examples → libraries → ADRs → interview notes → impact assessment.
It describes what is **built** (marked ✅) and what is **proposed** (marked 💡).

---

## 1. Current Project Analysis (the legacy `TezRota` code)

| Area | Finding |
|---|---|
| Stack | WinForms, .NET Framework 4.8, old-style `.csproj` with `packages.config`, LINQ to SQL, ClosedXML |
| Structure | `ModelKatmani` (5 POCOs), `VeriKatmani` (generated LINQ to SQL, 1,394 lines), `YoneticiKatmani` (2 manager classes, 725 lines), `YardimciKatmani`, one form `AnaEkran` |
| What the code does | Vehicle/shipment routing for a heterogeneous fleet: one hard-coded depot (point id 65, Eskişehir), haversine distances, **Simulated Annealing only**, data from SQL Server, plans imported from Excel |
| What the README describes | A different program: 20-point TSP, Euclidean distance, **Simulated Annealing and Tabu Search**, one screen (screenshots from January 2019). Tabu Search and `CityPoint` never appear in the git history of this repository |

**Weaknesses and technical debt**

- *Architecture*: no separation between algorithm and data access. `CozumYonetici` reads the database inside the solver (`TasimaListesiniOku`, `SonucBilginiYaz`), so the algorithm cannot be run or tested without SQL Server. `PlanYonetici` takes WinForms controls (`ComboBox`, `DataGridView`) and shows `MessageBox`es.
- *Hard-coded environment*: server name `DESKTOP-PLGLH0G`, `C:\Users\NisanurrunasiN\Desktop\test`, depot id 65 and its coordinates.
- *Correctness*: `NakliyeSkorHesapla` returns the last score, not the maximum; `Dispose()` throws `NotImplementedException`; the SA loop mutates the input list while building its neighbour; large blocks of commented-out code.
- *Naming*: Turkish/English mix (`RotaList`, `NakSkor`, `ELMyeOlanUZaklik`, `rastgele`), public fields, inconsistent casing.
- *UI*: ratio-based `TableLayoutPanel` layout, default grey controls, results as plain text, a batch loop inside a button handler (`BtnPlanRotala_Click`), no live drawing.
- *Performance/scalability*: LINQ lookups inside the cost function (`NoktaListesi.Where(...)` per distance), a database write per row, synchronous work on the UI thread.
- *Maintainability*: no tests, no CI, no configuration, no logging.

**Modernization opportunities** – all taken up by Phileas: SDK-style projects, .NET 10, nullable reference types, records,
file-scoped namespaces, collection expressions, primary constructors, `async`/`await`, dependency injection, options
pattern, logging, unit tests, CI.

## 2. Modernization Plan

| Opportunity | Where it is used | Why it helps |
|---|---|---|
| .NET 10 (LTS), C# 14 | all projects | current runtime, long support window (.NET 8 support ends Nov 2026) |
| SDK-style projects, no `packages.config` | all | readable `.csproj`, `dotnet build` works everywhere, CI-friendly |
| Nullable reference types + `TreatWarningsAsErrors` | `src/Directory.Build.props` | null bugs surface at compile time |
| Records (`City`, `Snapshot`, `SolveResult`, settings) | `Phileas.Core/Models` | immutable value-like data, `with` copies, equality for free |
| Collection expressions (`[]`, `[.. x]`) | Core, ViewModels | shorter, one syntax for arrays/lists/spreads |
| Primary constructors | solvers, ViewModels, services | removes constructor/field boilerplate; the dependency list is visible in the type header |
| Partial properties with `[ObservableProperty]` (MVVM Toolkit 8.4) | ViewModels | source-generated `INotifyPropertyChanged`, no backing-field noise |
| `required` / `init` properties | `SolveResult`, settings | an incomplete result cannot be constructed |
| `async` + `CancellationToken` | `OptimizationService`, `AlgorithmViewModel` | UI never blocks; every run can be cancelled |
| `Microsoft.Extensions.Hosting` (DI, configuration, logging) | `App.xaml.cs` | same composition root as ASP.NET Core; testable ViewModels |
| `IOptions<T>` + `appsettings.json` | `OptimizationDefaults` | defaults (100 / 1000 / 0.99 / 3 / 12) live in configuration, not in code |
| `ArgumentOutOfRangeException.ThrowIf…` helpers | solvers | concise guard clauses |
| xUnit tests + GitHub Actions | `tests/`, `.github/workflows` | every push builds and tests on Windows |

## 3. Step-by-Step Migration Roadmap

1. ✅ **Decide scope**: rewrite the Core instead of porting it (see ADR-1). Keep the WinForms project untouched.
2. ✅ **`Phileas.Core`**: `City`, `Tour`, `SimulatedAnnealing`, `TabuSearch`, `SolveResult` with replayable snapshots.
3. ✅ **`Phileas.Core.Tests`**: permutation validity, improvement over the random start, determinism with a seed, cancellation, guard clauses.
4. ✅ **WPF shell**: host/DI, `appsettings.json`, theme, one window.
5. ✅ **ViewModels**: `MainViewModel`, `AlgorithmViewModel` (+ `AnnealingViewModel`, `TabuViewModel`), `ResultViewModel`.
6. ✅ **Visualization**: `RouteView`, `ConvergenceView`, replay of recorded improvements.
7. ✅ **CI/CD**: build + test on Windows, PR labelling, automatic `vMAJOR.MINOR.PATCH` releases.
8. 💡 **Phase 2 – .NET MAUI** (Windows + Android): move the ViewModels to `Phileas.ViewModels`, draw with `GraphicsView`/`IDrawable`.
9. 💡 Optional: Serilog file sink, results export (CSV/PNG), custom start tour by clicking, 2-opt neighbourhood as a third solver.

## 4. Architecture Design

Two production projects and one test project. Layering is kept deliberately flat.

```
┌────────────────────────────── Phileas.Wpf (net10.0-windows) ──────────────────────────────┐
│  Views (XAML)  ──binds──►  ViewModels  ──uses──►  Services (IOptimizationService)          │
│  Controls: RouteView, ConvergenceView, LabeledSlider        Infrastructure: OptimizationDefaults │
│  Composition root: App.xaml.cs → Host (DI + configuration + logging)                        │
└───────────────────────────────────────────┬────────────────────────────────────────────────┘
                                            │ project reference
┌─────────────────────────── Phileas.Core (net10.0, no NuGet packages) ──────────────────────┐
│  Models: City, AnnealingSettings, TabuSettings, SolveResult, Snapshot, CityGenerator        │
│  Algorithms: IRouteSolver ◄── SimulatedAnnealing, TabuSearch;  Tour (distance helpers)      │
└─────────────────────────────────────────────────────────────────────────────────────────────┘
```

**Rules**: Core knows nothing about WPF, files or databases. ViewModels contain no WPF types (no `Brush`, no
`Visibility`), so they can move to a shared library for MAUI. Views contain no logic beyond view concerns
(`LabeledSlider` formats a number; `RouteView` draws).

**Data flow of one run**

1. The user presses *Compare*: `MainViewModel.CompareCommand` → `RunAsync(Annealing, Tabu)`.
2. Each `AlgorithmViewModel.RunAsync` calls `IOptimizationService.SolveAsync`, which runs the solver on a thread-pool thread.
3. The solver returns a `SolveResult` including up to 300 `Snapshot`s (every improvement of the best tour).
4. The ViewModel replays the snapshots over *Animation* seconds on the UI thread; bindings redraw `RouteView` and `ConvergenceView`.
5. `ResultViewModel` builds the comparison rows; the shortest tour is highlighted.

**Algorithms** (defaults from the thesis): both start from the same random tour; neighbour = swap of two cities; distance = Euclidean on a 1000 × 600 plane (1 unit = 1 km); travel time = distance / speed.

- *Simulated Annealing*: accept a shorter tour always, a longer one with probability `exp(-Δ/T)`; `T ← T · 0.99` after every 1000 iterations until `T < 1` (≈ 459,000 iterations).
- *Tabu Search*: every iteration evaluates 12 random swap neighbours and moves to the best non-tabu one; swap A-B equals B-A; FIFO list of length 3; aspiration (a tabu move is allowed if it beats the best tour).

## 5. Folder Structure

```
TezRota/                          repo root (name may become "Phileas")
├── Phileas.sln                   solution of the new code
├── TezRota.sln, TezRota.csproj   legacy WinForms application, untouched
├── .github/
│   ├── workflows/                dotnet.yml (CI), release.yml (versioning), auto-assign.yml
│   └── pull_request_template.md
├── docs/                         this document, screenshots, original Turkish README
├── src/
│   ├── Directory.Build.props     Nullable, ImplicitUsings, LangVersion, warnings as errors
│   ├── Phileas.Core/
│   │   ├── Models/               City, settings, results, CityGenerator
│   │   └── Algorithms/           IRouteSolver, SimulatedAnnealing, TabuSearch, Tour, SnapshotRecorder
│   └── Phileas.Wpf/
│       ├── Views/                MainWindow, AlgorithmPanel (+ a tiny converter)
│       ├── ViewModels/           MainViewModel, AlgorithmViewModel, AnnealingViewModel, TabuViewModel, ResultViewModel, Display
│       ├── Services/             IOptimizationService, OptimizationService
│       ├── Controls/             RouteView, ConvergenceView, LabeledSlider
│       ├── Infrastructure/       OptimizationDefaults (options class)
│       ├── Themes/               Theme.xaml (design tokens, card/button styles)
│       ├── Assets/               Logo.xaml (vector logo), phileas.ico, splash.png
│       ├── appsettings.json      default parameters, log level
│       └── App.xaml(.cs)         theme + composition root
└── tests/Phileas.Core.Tests/     xUnit
```

Each folder answers one question: *Views* – what is on screen; *ViewModels* – what the screen knows and can do;
*Services* – work that needs a seam for testing/logging; *Controls* – reusable drawing/input elements;
*Infrastructure* – configuration types; *Themes* – look and feel. A `Converters` folder was not created because
only one converter is needed.

## 6. UI/UX Redesign

**Style**: light, map-inspired Fluent look (built-in `ThemeMode="Light"` of .NET 10 WPF) with a gradient banner, soft colour washes, translucent rounded
cards, soft shadows, gradient logo and primary button, Segoe Fluent icons, vector logo drawn as XAML paths.

**Wireframe (single window, 1240 × 720 and up)**

```
┌──────────────────────────────────────────────────────────────────────────────────────────┐
│ [logo] Phileas                                                                             │
│        Travelling salesman · Simulated Annealing vs Tabu Search                            │
├───────────────┬──────────────────────────────────────┬─────────────────────────────────────┤
│ ┌ Problem ──┐ │ ┌ ● Simulated Annealing  [status] ┐ │ ┌ ● Tabu Search        [status] ┐   │
│ │ Cities    │ │ │  ┌───────────────────────────┐  │ │ │  ┌───────────────────────────┐ │   │
│ │ Speed     │ │ │  │      live route canvas    │  │ │ │  │      live route canvas    │ │   │
│ │ Animation │ │ │  └───────────────────────────┘  │ │ │  └───────────────────────────┘ │   │
│ │ [New city]│ │ │  ━━━━ progress                  │ │ │  ━━━━ progress                 │   │
│ └───────────┘ │ │  Distance Travel Iteration Solve │ │ │  Distance Travel Iteration ... │   │
│ ┌ Annealing ▷┐│ │  O→H→N→Q→…                       │ │ │  R→F→M→…                       │   │
│ │ sliders   ││ └──────────────────────────────────┘ │ └────────────────────────────────┘   │
│ └───────────┘ │ ┌ Convergence ───────────────────┐   ┌ Results ──────────────────────────┐ │
│ ┌ Tabu      ▷┐│ │ best distance vs iterations    │   │ method | km | time | solve | gain │ │
│ │ sliders   ││ └────────────────────────────────┘   └───────────────────────────────────┘ │
│ [ Compare both methods ]                                                                    │
└───────────────┴──────────────────────────────────────┴─────────────────────────────────────┘
```

**Layout decisions**: parameters on the left (cause), the two live routes side by side in the centre (effect, same
scale so they are directly comparable), numbers below the routes, convergence and results at the bottom.
The two methods have fixed colours everywhere (Atlas blue = annealing, copper = tabu, map green = shortest tour) so charts, chips and routes read
at a glance. Sliders instead of text boxes make every input valid by construction, so no validation UI is needed.
The window opens at 1240 × 720 because the target laptop screens are about 1230 × 690 DIPs at 125 % scaling.

**Screens** (the request lists five; they map to regions of the one window, as in the original application):
Dashboard = the window; Configuration panel = left column; Visualization area = the two route cards;
Results area = the results card; Statistics panel = the four numbers under each route plus the convergence chart.

## 7. Visualization Design

✅ built:

- **Record and replay**. A solver finishes in tens to hundreds of milliseconds, which is too fast to watch. It records every improvement of the best tour; the UI replays the recording over a configurable time (1–10 s). The replay is time-based (`Stopwatch`), so it takes the same time on every machine.
- **Live route** (`RouteView`): glow + line + direction arrows, start city highlighted, city labels, dotted background grid.
- **Never frozen** (`RouteView.IsActive`): while a method is busy, light dots flow along the route and the start city emits pulse rings (driven by `CompositionTarget.Rendering`); the progress bar is indeterminate while searching and a determinate replay bar afterwards; the status chip reads *Searching…* → *Improving route* → *“x % shorter”*.
- **Live numbers**: current distance, travel time at the chosen speed, iteration counter, solve time (algorithm only, as in the thesis).
- **Convergence chart** (`ConvergenceView`): best distance vs. share of iterations, step lines that grow with the replay; the y-axis is zoomed on the interesting part.

💡 ideas: draw the route segment by segment with an animated `StrokeDashOffset`; show the *current* (not only the best) tour of SA as a faint second line; hover on a city to show its neighbours; temperature gauge for SA and a tabu-list strip for Tabu Search; click the canvas to add/move cities; export the route as PNG.

**Loader strategy in WPF**: (1) all heavy work on `Task.Run`; (2) `CancellationToken` wired to a Cancel button; (3) an animation that does not depend on the work (`CompositionTarget.Rendering`, `Storyboard`, indeterminate `ProgressBar`); (4) a status text that names the phase. Lottie is intentionally not used (see §9).

## 8. MVVM Implementation Examples

The real files are in `src/Phileas.Wpf/ViewModels`; the essential patterns:

```csharp
// Observable state without boilerplate (partial properties, MVVM Toolkit 8.4+)
[ObservableProperty]
[NotifyPropertyChangedFor(nameof(DistanceText), nameof(TravelTimeText))]
public partial double DisplayedDistance { get; private set; }

// Commands: async, with CanExecute coupled to a property
[ObservableProperty]
[NotifyCanExecuteChangedFor(nameof(CompareCommand), nameof(CancelCommand))]
public partial bool IsRunning { get; private set; }

[RelayCommand(CanExecute = nameof(CanStart))]
private Task CompareAsync() => RunAsync(Annealing, Tabu);

// Composition root (App.xaml.cs)
builder.Services.Configure<OptimizationDefaults>(builder.Configuration.GetSection("Defaults"));
builder.Services.AddSingleton<IOptimizationService, OptimizationService>();
builder.Services.AddSingleton<MainViewModel>();
builder.Services.AddSingleton<MainWindow>();
```

| Type | Responsibility |
|---|---|
| `MainViewModel` | owns the problem (cities, start tour, speed, animation time), creates the two algorithm ViewModels and the result table, exposes *Generate / Compare / Run / Cancel* |
| `AlgorithmViewModel` (abstract) | one method on screen: state machine `Idle → Solving → Replaying → Done`, replay, formatted numbers; subclasses add parameters and `CreateSolver()` |
| `AnnealingViewModel`, `TabuViewModel` | the parameters of one method and the factory for its solver |
| `ResultViewModel` | the comparison rows and the “shortest” marker |
| `IOptimizationService` | runs a solver off the UI thread, logs start/finish; the seam for fakes in tests |

Views get their `DataContext` from the container (`MainWindow(MainViewModel)`); `AlgorithmPanel` is a reusable
`UserControl` bound to an `AlgorithmViewModel`, so the two panels share one template.

## 9. Recommended Libraries

**Used (kept minimal)**

| Library | Why |
|---|---|
| CommunityToolkit.Mvvm | Microsoft-maintained, source generators, no runtime reflection, the de-facto standard for new MVVM code |
| Microsoft.Extensions.Hosting | DI + configuration + logging with the same API as ASP.NET Core |
| xUnit | standard test framework for .NET |

**Considered, not used** – and when to reconsider

| Need | Option | Decision |
|---|---|---|
| Theme/controls | **WPF-UI** (Fluent, Mica, navigation), ModernWpf, MaterialDesignInXaml | Not needed: the built-in Fluent light theme plus ~120 lines of `Theme.xaml` is enough for one window, keeps the repo dependency-free and shows WPF styling skill. Reconsider WPF-UI for a multi-page app |
| Charts | **LiveCharts2**, ScottPlot, OxyPlot | One small line chart is ~120 lines of custom drawing, and it grows with the replay. Reconsider for many series/zooming |
| Lottie | LottieSharp / SkiaSharp-based players | Adds Skia and a JSON asset for something a 20-line animation does; reconsider only with a designer-made animation |
| SVG | `SharpVectors`, or paths in XAML | Logo and icons are XAML paths/Segoe Fluent Icons; SharpVectors if external SVG files are needed |
| Icons | Segoe Fluent Icons (built into Windows 11), `FluentIcons.WPF`, Material.Icons | Segoe Fluent Icons with a Segoe MDL2 fallback |
| Logging | Serilog (file sink) | Microsoft.Extensions.Logging is enough; add Serilog if logs must be written to files |
| Animation | built-in `Storyboard`, `CompositionTarget.Rendering` | sufficient |

## 10. Architecture Decision Records

**ADR-1 – Rewrite the algorithm library instead of porting the legacy managers.**
*Context*: the legacy solver is tied to SQL Server, Excel and a fleet-routing model, contains known defects, and does not implement the TSP/Tabu Search that the README describes.
*Decision*: write `Phileas.Core` from scratch for the TSP, port only the annealing logic (swap neighbour, Metropolis acceptance, geometric cooling).
*Consequences*: small, testable, database-free core; the legacy project remains as history; the README states this openly.

**ADR-2 – Two projects (`Core`, `Wpf`) plus tests, not Clean-Architecture layers.**
*Decision*: the only boundary that matters is “algorithm vs. UI”. Application/Domain/Infrastructure projects would be empty shells.
*Consequences*: easy to explain; a `Phileas.ViewModels` project is added only when MAUI needs it.

**ADR-3 – Record and replay instead of live progress callbacks.**
*Context*: 20-city runs take milliseconds.
*Decision*: solvers return snapshots; the UI replays them at a chosen speed.
*Consequences*: deterministic, watchable, no `IProgress`/dispatcher plumbing inside algorithms; the run is not literally “live” – documented in the UI as *Animation* time.

**ADR-4 – Built-in Fluent theme and own styles, no UI library.** See §9.

**ADR-5 – Custom `FrameworkElement` drawing for route and chart.** Full control of glow, arrows and the reveal animation; no dependency; small. Trade-off: no built-in zoom/tooltips.

**ADR-6 – Host builder for DI/config/logging.** Familiar composition root, `IOptions` for defaults. Trade-off: one more package than `new ServiceCollection()`, accepted for configuration + logging.

**ADR-7 – Version numbers `vMAJOR.MINOR.PATCH` created by CI.** A merged PR gets the next tag: branch name containing `major` (or label `breaking`) → major, `dev` (label `enhancement`) → minor, anything else (e.g. `hotfix`) → patch. Numbering starts at `v0.0.0`. Direct pushes to `master` do not release.

## 11. How to Explain This Project in a Technical Interview

**30-second version.** “Phileas compares two metaheuristics for the travelling salesman problem. The algorithms live in a UI-free .NET library with unit tests; a WPF/MVVM front end runs them on a background thread and replays the search so you can watch the route improve and compare both methods on the same cities. It is the modern rewrite of my master's thesis application.”

**Talking points**

- *Architecture*: two projects; Core has zero dependencies; ViewModels have no WPF types, so they could be reused in MAUI. Dependency injection via the generic host, defaults via `IOptions`.
- *MVVM*: CommunityToolkit.Mvvm source generators; commands with `CanExecute` coupled to state; one abstract `AlgorithmViewModel` and two small subclasses (strategy-like); a reusable `AlgorithmPanel` view bound to it.
- *Concurrency*: `Task.Run` + `CancellationToken`; the replay loop is async on the UI thread and never blocks; the elapsed time shown is algorithm time only.
- *Algorithms*: why swap neighbourhood and Metropolis acceptance; what the temperature and cooling rate do; what the tabu list prevents (cycling) and why aspiration exists; why the two methods are compared from the same start tour and with a seed for reproducibility; the honest result: with the thesis defaults annealing finds shorter tours but takes longer than tabu search.
- *Modernization*: why records, primary constructors, partial properties, nullable annotations, warnings as errors.
- *Engineering practice*: tests for the algorithms, GitHub Actions build/test, semantic-style release tags from PR branches.
- *Trade-offs you can discuss*: custom drawing vs. chart library; recorded replay vs. live callbacks; no UI library; swap vs. 2-opt neighbourhood (2-opt would give shorter tours; swap was kept to match the thesis).
- *Candid history*: the legacy code solved a different (fleet-routing, SQL-based) problem than the README describes; Phileas implements the README's problem cleanly.

## 12. Recruiter Impact Assessment

| Signal | Evidence in the repo |
|---|---|
| WPF, MVVM, modern .NET | `src/Phileas.Wpf`: styled, data-bound window, custom controls, source-generated MVVM, DI, options, logging, .NET 10 / C# 14 |
| Clean code | small files, consistent English naming, no dead code, nullable + warnings-as-errors |
| Testing and CI | xUnit tests on the algorithms, Windows CI, release automation |
| Algorithms | two metaheuristics with documented parameters, reproducible runs, visual comparison |
| Product sense | screenshot-ready colourful UI, live animation, results that make the comparison obvious |
| Communication | this document (ADRs), English README with a German summary, honest note about the legacy code |

**Remaining gaps** (so you can name them yourself first): no MAUI project yet (phase 2); no UI automation tests; the UI is a single window; the replay is a visualization of a finished run, not a streamed one; no installer/publish pipeline.
