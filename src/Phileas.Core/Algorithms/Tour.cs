using Phileas.Core.Models;

namespace Phileas.Core.Algorithms;

/// <summary>Helpers for a tour, which is an array of city indices visited in order and closed back to the start.</summary>
public static class Tour
{
    public static double Distance(IReadOnlyList<City> cities, int[] order)
    {
        double total = 0;
        for (var i = 0; i < order.Length; i++)
        {
            total += cities[order[i]].DistanceTo(cities[order[(i + 1) % order.Length]]);
        }
        return total;
    }

    public static int[] RandomOrder(int count, Random random)
    {
        int[] order = [.. Enumerable.Range(0, count)];
        random.Shuffle(order);
        return order;
    }

    public static TimeSpan TravelTime(double distanceKm, double speedKmPerHour) =>
        speedKmPerHour > 0 ? TimeSpan.FromHours(distanceKm / speedKmPerHour) : TimeSpan.Zero;

    public static string Describe(IReadOnlyList<City> cities, int[] order) =>
        string.Join("→", order.Append(order[0]).Select(i => cities[i].Name));
}
