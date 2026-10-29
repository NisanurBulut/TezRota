using CommunityToolkit.Mvvm.ComponentModel;
using Phileas.Core.Algorithms;
using Phileas.Core.Models;
using Phileas.Wpf.Services;

namespace Phileas.Wpf.ViewModels;

public sealed partial class AnnealingViewModel(IOptimizationService optimizer, AnnealingSettings defaults) : AlgorithmViewModel(optimizer)
{
    public override string Name => "Simulated Annealing";
    public override string Description => "Accepts worse routes at first, less and less as it cools.";

    [ObservableProperty]
    public partial double Temperature { get; set; } = defaults.Temperature;

    [ObservableProperty]
    public partial int Iterations { get; set; } = defaults.Iterations;

    [ObservableProperty]
    public partial double CoolingRate { get; set; } = defaults.CoolingRate;

    protected override IRouteSolver CreateSolver() =>
        new SimulatedAnnealing(new AnnealingSettings { Temperature = Temperature, Iterations = Iterations, CoolingRate = CoolingRate });
}
