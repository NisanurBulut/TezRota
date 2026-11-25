namespace Phileas.Core.Models;

/// <summary>
/// A point of the travelling salesman problem on a flat plane (1 unit = 1 km).
/// In a <see cref="Warehouse"/> it is a pick location and the distance is walked along the aisles.
/// </summary>
public sealed record City(int Id, string Name, double X, double Y, bool InWarehouse = false)
{
    public double DistanceTo(City other) => InWarehouse
        ? Warehouse.WalkDistance(this, other)
        : Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
}
