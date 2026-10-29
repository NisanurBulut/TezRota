using CommunityToolkit.Mvvm.ComponentModel;
using Phileas.Core.Algorithms;
using Phileas.Core.Models;
using Phileas.Wpf.Services;

namespace Phileas.Wpf.ViewModels;

public sealed partial class TabuViewModel(IOptimizationService optimizer, TabuSettings defaults) : AlgorithmViewModel(optimizer)
{
    public override string Name => "Tabu Search";
    public override string Description => "Always takes the best neighbour and remembers recent moves to avoid cycles.";

    [ObservableProperty]
    public partial int TabuLength { get; set; } = defaults.TabuLength;

    [ObservableProperty]
    public partial int NeighbourCount { get; set; } = defaults.NeighbourCount;

    [ObservableProperty]
    public partial int Iterations { get; set; } = defaults.Iterations;

    protected override IRouteSolver CreateSolver() =>
        new TabuSearch(new TabuSettings { TabuLength = TabuLength, NeighbourCount = NeighbourCount, Iterations = Iterations });
}
