using Phileas.Core.Models;

namespace Phileas.Core.Algorithms;

/// <summary>Remembers each improvement of the best tour so the UI can replay the search later.</summary>
internal sealed class SnapshotRecorder(int maxCount = 300)
{
    private readonly List<Snapshot> _snapshots = [];

    public void Add(long iteration, double distance, int[] order) =>
        _snapshots.Add(new Snapshot(iteration, distance, [.. order]));

    /// <summary>Keeps at most <c>maxCount</c> evenly spread snapshots; the first and the last are always kept.</summary>
    public IReadOnlyList<Snapshot> ToList()
    {
        if (_snapshots.Count <= maxCount)
        {
            return _snapshots;
        }

        var step = (double)(_snapshots.Count - 1) / (maxCount - 1);
        return [.. Enumerable.Range(0, maxCount).Select(i => _snapshots[(int)Math.Round(i * step)])];
    }
}
