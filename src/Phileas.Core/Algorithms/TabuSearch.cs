using System.Diagnostics;
using Phileas.Core.Models;

namespace Phileas.Core.Algorithms;

/// <summary>
/// Tabu search: every iteration creates <see cref="TabuSettings.NeighbourCount"/> random swap neighbours and moves
/// to the best one that is not tabu. A swap of cities A and B is remembered in a FIFO list (A-B equals B-A) and
/// forbidden until it drops out of the list; a tabu move is still allowed if it beats the best tour so far.
/// </summary>
public sealed class TabuSearch(TabuSettings settings) : IRouteSolver
{
    public string Name => "Tabu Search";

    public SolveResult Solve(IReadOnlyList<City> cities, int[] startOrder, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(cities.Count, 3);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(settings.Iterations);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(settings.NeighbourCount);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(settings.TabuLength);

        var random = settings.Seed is { } seed ? new Random(seed) : new Random();
        var stopwatch = Stopwatch.StartNew();

        var current = (int[])startOrder.Clone();
        var currentDistance = Tour.Distance(cities, current);
        var best = (int[])current.Clone();
        var bestDistance = currentDistance;
        var startDistance = currentDistance;

        var recorder = new SnapshotRecorder();
        recorder.Add(0, bestDistance, best);

        var tabuList = new Queue<(int CityA, int CityB)>();

        for (var iteration = 1; iteration <= settings.Iterations; iteration++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            (int I, int J, double Distance)? bestMove = null;
            for (var n = 0; n < settings.NeighbourCount; n++)
            {
                var i = random.Next(current.Length);
                var j = random.Next(current.Length);
                if (i == j)
                {
                    continue;
                }

                (current[i], current[j]) = (current[j], current[i]);
                var candidateDistance = Tour.Distance(cities, current);
                (current[i], current[j]) = (current[j], current[i]);

                var isTabu = tabuList.Contains(MoveKey(current[i], current[j]));
                var beatsBest = candidateDistance < bestDistance;
                if ((!isTabu || beatsBest) && (bestMove is null || candidateDistance < bestMove.Value.Distance))
                {
                    bestMove = (i, j, candidateDistance);
                }
            }

            if (bestMove is not { } move)
            {
                continue; // every neighbour was tabu
            }

            tabuList.Enqueue(MoveKey(current[move.I], current[move.J]));
            if (tabuList.Count > settings.TabuLength)
            {
                tabuList.Dequeue();
            }

            (current[move.I], current[move.J]) = (current[move.J], current[move.I]);
            currentDistance = move.Distance;

            if (currentDistance < bestDistance)
            {
                bestDistance = currentDistance;
                best = (int[])current.Clone();
                recorder.Add(iteration, bestDistance, best);
            }
        }

        stopwatch.Stop();
        return new SolveResult
        {
            Method = Name,
            Order = best,
            Distance = bestDistance,
            StartDistance = startDistance,
            Elapsed = stopwatch.Elapsed,
            Iterations = settings.Iterations,
            Snapshots = recorder.ToList(),
        };
    }

    private static (int, int) MoveKey(int cityA, int cityB) => cityA < cityB ? (cityA, cityB) : (cityB, cityA);
}
