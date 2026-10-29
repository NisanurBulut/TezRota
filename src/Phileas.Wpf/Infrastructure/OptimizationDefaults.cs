using Phileas.Core.Models;

namespace Phileas.Wpf.Infrastructure;

/// <summary>Start values of the controls; bound from the "Defaults" section of appsettings.json.</summary>
public sealed class OptimizationDefaults
{
    public int CityCount { get; init; } = 20;
    public double SpeedKmPerHour { get; init; } = 60;

    /// <summary>How long the recorded search is replayed on screen.</summary>
    public double AnimationSeconds { get; init; } = 3;

    public AnnealingSettings Annealing { get; init; } = new();
    public TabuSettings Tabu { get; init; } = new();
}
