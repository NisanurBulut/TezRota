using Microsoft.Extensions.Logging;
using Phileas.Core.Algorithms;
using Phileas.Core.Models;

namespace Phileas.Wpf.Services;

public interface IOptimizationService
{
    Task<SolveResult> SolveAsync(IRouteSolver solver, IReadOnlyList<City> cities, int[] startOrder, CancellationToken cancellationToken);
}

/// <summary>Runs a solver on a background thread so the window never freezes.</summary>
public sealed class OptimizationService(ILogger<OptimizationService> logger) : IOptimizationService
{
    public async Task<SolveResult> SolveAsync(IRouteSolver solver, IReadOnlyList<City> cities, int[] startOrder, CancellationToken cancellationToken)
    {
        logger.LogInformation("{Solver} started on {Count} cities", solver.Name, cities.Count);

        var result = await Task.Run(() => solver.Solve(cities, startOrder, cancellationToken), cancellationToken);

        logger.LogInformation(
            "{Solver} finished: {Distance:F1} km in {Elapsed:F0} ms ({Improvement:F1}% shorter than the start tour)",
            solver.Name, result.Distance, result.Elapsed.TotalMilliseconds, result.ImprovementPercent);
        return result;
    }
}
