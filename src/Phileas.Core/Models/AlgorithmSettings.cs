namespace Phileas.Core.Models;

/// <summary>Parameters of simulated annealing. Defaults are the values of the original thesis application.</summary>
public sealed record AnnealingSettings
{
    public double Temperature { get; init; } = 100;

    /// <summary>Iterations per temperature step (the temperature is cooled until it drops below 1).</summary>
    public int Iterations { get; init; } = 1000;

    public double CoolingRate { get; init; } = 0.99;

    /// <summary>Fixed seed for reproducible runs; null picks a random one.</summary>
    public int? Seed { get; init; }
}

/// <summary>Parameters of tabu search. Defaults are the values of the original thesis application.</summary>
public sealed record TabuSettings
{
    public int TabuLength { get; init; } = 3;
    public int NeighbourCount { get; init; } = 12;
    public int Iterations { get; init; } = 1000;
    public int? Seed { get; init; }
}
