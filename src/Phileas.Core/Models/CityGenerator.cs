namespace Phileas.Core.Models;

public static class CityGenerator
{
    public const double Width = 1000;
    public const double Height = 600;
    private const double Margin = 40;

    public static IReadOnlyList<City> Create(int count, int? seed = null)
    {
        var random = seed is { } s ? new Random(s) : new Random();
        return
        [
            .. Enumerable.Range(0, count).Select(i => new City(
                i,
                i < 26 ? ((char)('A' + i)).ToString() : $"P{i + 1}",
                Margin + random.NextDouble() * (Width - 2 * Margin),
                Margin + random.NextDouble() * (Height - 2 * Margin))),
        ];
    }
}
