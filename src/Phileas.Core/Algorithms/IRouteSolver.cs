using Phileas.Core.Models;

namespace Phileas.Core.Algorithms;

public interface IRouteSolver
{
    string Name { get; }

    /// <summary>Improves <paramref name="startOrder"/>; the input array is not modified.</summary>
    SolveResult Solve(IReadOnlyList<City> cities, int[] startOrder, CancellationToken cancellationToken = default);
}
