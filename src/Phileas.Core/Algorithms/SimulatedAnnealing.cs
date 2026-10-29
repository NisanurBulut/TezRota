using System.Diagnostics;
using Phileas.Core.Models;

namespace Phileas.Core.Algorithms;

/// <summary>
/// Simulated annealing: swap two random cities; keep the result if it is shorter, or, with probability
/// exp(-delta / temperature), even if it is longer. The temperature is multiplied by the cooling rate after
/// every <see cref="AnnealingSettings.Iterations"/> iterations until it drops below 1.
/// </summary>
public sealed class SimulatedAnnealing(AnnealingSettings settings) : IRouteSolver
{
    private const double MinTemperature = 1;

    public string Name => "Simulated Annealing";

    public SolveResult Solve(IReadOnlyList<City> cities, int[] startOrder, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(cities.Count, 3);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(settings.Iterations);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(settings.Temperature);
        if (settings.CoolingRate is <= 0 or >= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(settings), "Cooling rate must be between 0 and 1 (exclusive).");
        }

        var random = settings.Seed is { } seed ? new Random(seed) : new Random();
        var stopwatch = Stopwatch.StartNew();

        var current = (int[])startOrder.Clone();
        var currentDistance = Tour.Distance(cities, current);
        var best = (int[])current.Clone();
        var bestDistance = currentDistance;
        var startDistance = currentDistance;

        var recorder = new SnapshotRecorder();
        recorder.Add(0, bestDistance, best);

        long iteration = 0;
        for (var temperature = settings.Temperature; temperature > MinTemperature; temperature *= settings.CoolingRate)
        {
            cancellationToken.ThrowIfCancellationRequested();

            for (var k = 0; k < settings.Iterations; k++, iteration++)
            {
                var i = random.Next(current.Length);
                var j = random.Next(current.Length);
                (current[i], current[j]) = (current[j], current[i]);

                var candidateDistance = Tour.Distance(cities, current);
                var delta = candidateDistance - currentDistance;

                if (delta <= 0 || random.NextDouble() < Math.Exp(-delta / temperature))
                {
                    currentDistance = candidateDistance;
                    if (currentDistance < bestDistance)
                    {
                        bestDistance = currentDistance;
                        best = (int[])current.Clone();
                        recorder.Add(iteration, bestDistance, best);
                    }
                }
                else
                {
                    (current[i], current[j]) = (current[j], current[i]); // undo
                }
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
            Iterations = iteration,
            Snapshots = recorder.ToList(),
        };
    }
}
