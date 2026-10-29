using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Phileas.Core.Algorithms;
using Phileas.Core.Models;
using Phileas.Wpf.Services;

namespace Phileas.Wpf.ViewModels;

public enum AlgorithmState
{
    Idle,
    Solving,
    Replaying,
    Done,
}

/// <summary>
/// One solving method on screen: its parameters (in the subclass), its progress and the tour currently drawn.
/// A run first solves on a background thread and then replays the recorded improvements so they can be watched.
/// </summary>
public abstract partial class AlgorithmViewModel(IOptimizationService optimizer) : ObservableObject
{
    [ObservableProperty]
    public partial IReadOnlyList<City> Cities { get; private set; } = [];

    public abstract string Name { get; }
    public abstract string Description { get; }

    protected abstract IRouteSolver CreateSolver();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBusy), nameof(StatusText))]
    public partial AlgorithmState State { get; private set; }

    /// <summary>The tour drawn right now: the start tour, then every improvement during the replay.</summary>
    [ObservableProperty]
    public partial int[] DisplayedOrder { get; private set; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DistanceText), nameof(TravelTimeText))]
    public partial double DisplayedDistance { get; private set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IterationText))]
    public partial long DisplayedIteration { get; private set; }

    [ObservableProperty]
    public partial string RouteText { get; private set; } = "";

    [ObservableProperty]
    public partial double ReplayProgress { get; private set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TravelTimeText))]
    public partial double SpeedKmPerHour { get; set; } = 60;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SolveTimeText), nameof(StatusText))]
    public partial SolveResult? Result { get; private set; }

    public bool IsBusy => State is AlgorithmState.Solving or AlgorithmState.Replaying;

    public string DistanceText => Display.Distance(DisplayedDistance);
    public string TravelTimeText => Display.TravelTime(Tour.TravelTime(DisplayedDistance, SpeedKmPerHour));
    public string IterationText => Display.Count(DisplayedIteration);
    public string SolveTimeText => Result is null ? "–" : Display.SolveTime(Result.Elapsed);

    public string StatusText => State switch
    {
        AlgorithmState.Solving => "Searching…",
        AlgorithmState.Replaying => "Improving route",
        AlgorithmState.Done => Result is null ? "Done" : $"{Result.ImprovementPercent:F1}% shorter",
        _ => "Ready",
    };

    /// <summary>Shows the unsolved start tour and forgets any earlier result.</summary>
    public void Reset(IReadOnlyList<City> cities, int[] startOrder)
    {
        Cities = cities;
        Result = null;
        ReplayProgress = 0;
        State = AlgorithmState.Idle;
        Show(startOrder, Tour.Distance(cities, startOrder), 0);
    }

    public async Task RunAsync(int[] startOrder, TimeSpan replayDuration, CancellationToken cancellationToken)
    {
        Reset(Cities, startOrder);
        try
        {
            State = AlgorithmState.Solving;
            var result = await optimizer.SolveAsync(CreateSolver(), Cities, startOrder, cancellationToken);

            Result = result;
            State = AlgorithmState.Replaying;
            await ReplayAsync(result, replayDuration, cancellationToken);

            State = AlgorithmState.Done;
        }
        catch (OperationCanceledException)
        {
            Reset(Cities, startOrder);
            throw;
        }
    }

    private async Task ReplayAsync(SolveResult result, TimeSpan duration, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var lastIndex = result.Snapshots.Count - 1;

        while (stopwatch.Elapsed < duration)
        {
            ReplayProgress = stopwatch.Elapsed / duration;
            var snapshot = result.Snapshots[(int)(ReplayProgress * lastIndex)];
            Show(snapshot.Order, snapshot.Distance, snapshot.Iteration);
            await Task.Delay(16, cancellationToken);
        }

        ReplayProgress = 1;
        Show(result.Order, result.Distance, result.Iterations);
    }

    private void Show(int[] order, double distance, long iteration)
    {
        DisplayedOrder = order;
        DisplayedDistance = distance;
        DisplayedIteration = iteration;
        RouteText = Tour.Describe(Cities, order);
    }
}
