namespace Phileas.Core.Models;

/// <summary>A point of the travelling salesman problem on a flat plane (1 unit = 1 km).</summary>
public sealed record City(int Id, string Name, double X, double Y)
{
    public double DistanceTo(City other) => Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
}
