namespace Phileas.Core.Models;

/// <summary>The best tour known after <see cref="Iteration"/> iterations; used to replay a run in the UI.</summary>
public sealed record Snapshot(long Iteration, double Distance, int[] Order);

public sealed record SolveResult
{
    public required string Method { get; init; }

    /// <summary>City indices in visiting order; the tour returns from the last city to the first.</summary>
    public required int[] Order { get; init; }

    public required double Distance { get; init; }
    public required double StartDistance { get; init; }

    /// <summary>Time spent inside the algorithm only (not drawing or replaying).</summary>
    public required TimeSpan Elapsed { get; init; }

    public required long Iterations { get; init; }

    /// <summary>Every improvement of the best tour, thinned to a few hundred entries; first = start tour, last = result.</summary>
    public required IReadOnlyList<Snapshot> Snapshots { get; init; }

    public double ImprovementPercent => StartDistance <= 0 ? 0 : (StartDistance - Distance) / StartDistance * 100;
}
