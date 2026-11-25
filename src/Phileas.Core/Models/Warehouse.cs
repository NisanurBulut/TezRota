namespace Phileas.Core.Models;

/// <summary>
/// A warehouse for order picking, drawn on the same 1000 × 600 plane as the cities (1 unit = 0.1 m, so 100 m × 60 m).
/// Racks stand on both sides of vertical aisles; a cross aisle at the top and one at the bottom connect the aisles.
/// A pick location lies in an aisle, so the picker walks along the aisles instead of in a straight line.
/// </summary>
public static class Warehouse
{
    public const double Top = 60;
    public const double Bottom = 540;
    public const double RackWidth = 40;
    private const double KmPerUnit = 0.0001;

    public static readonly double[] Aisles = [80, 200, 320, 440, 560, 680, 800, 920];

    /// <summary>The depot on the bottom cross aisle, then <paramref name="count"/> random pick locations.</summary>
    public static IReadOnlyList<City> Create(int count, int? seed = null)
    {
        var random = seed is { } s ? new Random(s) : new Random();
        var picks = Enumerable.Range(1, count).Select(i => new City(
            i,
            i <= 26 ? ((char)('A' + i - 1)).ToString() : $"P{i}",
            Aisles[random.Next(Aisles.Length)],
            Top + 45 + 30 * random.Next(14),
            InWarehouse: true));

        return [new City(0, "Depot", 20, Bottom, InWarehouse: true), .. picks];
    }

    /// <summary>Walking distance in km: straight along the same aisle, otherwise through the shorter cross aisle.</summary>
    public static double WalkDistance(City from, City to)
    {
        var units = from.X == to.X
            ? Math.Abs(from.Y - to.Y)
            : Math.Abs(from.X - to.X) + Math.Min(from.Y + to.Y - 2 * Top, 2 * Bottom - from.Y - to.Y);
        return units * KmPerUnit;
    }

    /// <summary>The corners of the walk to <paramref name="to"/>, ending at <paramref name="to"/>; used for drawing.</summary>
    public static IEnumerable<(double X, double Y)> Walk(City from, City to)
    {
        if (from.X != to.X)
        {
            var crossAisle = from.Y + to.Y - 2 * Top < 2 * Bottom - from.Y - to.Y ? Top : Bottom;
            yield return (from.X, crossAisle);
            yield return (to.X, crossAisle);
        }
        yield return (to.X, to.Y);
    }
}
