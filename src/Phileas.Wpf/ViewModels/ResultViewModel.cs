using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Phileas.Core.Algorithms;
using Phileas.Core.Models;

namespace Phileas.Wpf.ViewModels;

public sealed record ResultRow(string Method, string Distance, string TravelTime, string SolveTime, string Improvement, string Iterations, bool IsBest);

/// <summary>The comparison table: one row per finished method, the shortest tour is marked.</summary>
public sealed partial class ResultViewModel : ObservableObject
{
    public ObservableCollection<ResultRow> Rows { get; } = [];

    [ObservableProperty]
    public partial bool HasResults { get; private set; }

    public void Update(IEnumerable<SolveResult> results, double speedKmPerHour)
    {
        Rows.Clear();
        var finished = results.ToList();
        foreach (var result in finished)
        {
            Rows.Add(new ResultRow(
                result.Method,
                Display.Distance(result.Distance),
                Display.TravelTime(Tour.TravelTime(result.Distance, speedKmPerHour)),
                Display.SolveTime(result.Elapsed),
                $"{result.ImprovementPercent:F1} %",
                Display.Count(result.Iterations),
                IsBest: finished.Count > 1 && result.Distance <= finished.Min(r => r.Distance)));
        }
        HasResults = Rows.Count > 0;
    }
}
