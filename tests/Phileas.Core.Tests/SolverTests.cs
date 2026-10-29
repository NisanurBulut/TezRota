using Phileas.Core.Algorithms;
using Phileas.Core.Models;

namespace Phileas.Core.Tests;

public class SolverTests
{
    private static readonly IReadOnlyList<City> Cities = CityGenerator.Create(20, seed: 1);
    private static readonly int[] StartOrder = Tour.RandomOrder(Cities.Count, new Random(2));

    public static TheoryData<IRouteSolver> Solvers => new()
    {
        new SimulatedAnnealing(new AnnealingSettings { Seed = 3 }),
        new TabuSearch(new TabuSettings { Seed = 3 }),
    };

    [Theory]
    [MemberData(nameof(Solvers))]
    public void Result_visits_every_city_exactly_once(IRouteSolver solver)
    {
        var result = solver.Solve(Cities, StartOrder);

        Assert.Equal(Enumerable.Range(0, Cities.Count), result.Order.Order());
    }

    [Theory]
    [MemberData(nameof(Solvers))]
    public void Result_is_shorter_than_the_random_start_and_matches_its_order(IRouteSolver solver)
    {
        var result = solver.Solve(Cities, StartOrder);

        Assert.True(result.Distance < result.StartDistance);
        Assert.Equal(Tour.Distance(Cities, result.Order), result.Distance, precision: 6);
        Assert.Equal(Tour.Distance(Cities, StartOrder), result.StartDistance, precision: 6);
    }

    [Theory]
    [MemberData(nameof(Solvers))]
    public void Snapshots_start_at_the_start_tour_and_end_at_the_result(IRouteSolver solver)
    {
        var result = solver.Solve(Cities, StartOrder);

        Assert.Equal(result.StartDistance, result.Snapshots[0].Distance);
        Assert.Equal(result.Distance, result.Snapshots[^1].Distance);
        Assert.True(result.Snapshots.Count <= 300);
    }

    [Fact]
    public void Same_seed_gives_the_same_result()
    {
        var first = new SimulatedAnnealing(new AnnealingSettings { Seed = 7 }).Solve(Cities, StartOrder);
        var second = new SimulatedAnnealing(new AnnealingSettings { Seed = 7 }).Solve(Cities, StartOrder);

        Assert.Equal(first.Order, second.Order);
    }

    [Fact]
    public void Solver_does_not_modify_the_start_order()
    {
        int[] copy = [.. StartOrder];

        new TabuSearch(new TabuSettings { Seed = 3 }).Solve(Cities, StartOrder);

        Assert.Equal(copy, StartOrder);
    }

    [Fact]
    public void Cooling_rate_of_one_is_rejected()
    {
        var solver = new SimulatedAnnealing(new AnnealingSettings { CoolingRate = 1 });

        Assert.Throws<ArgumentOutOfRangeException>(() => solver.Solve(Cities, StartOrder));
    }

    [Fact]
    public void Cancellation_stops_the_search()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.Throws<OperationCanceledException>(() => new TabuSearch(new TabuSettings()).Solve(Cities, StartOrder, cts.Token));
    }

    [Fact]
    public void Square_tour_has_the_perimeter_as_distance()
    {
        City[] square = [new(0, "A", 0, 0), new(1, "B", 10, 0), new(2, "C", 10, 10), new(3, "D", 0, 10)];

        Assert.Equal(40, Tour.Distance(square, [0, 1, 2, 3]), precision: 6);
        Assert.Equal("A→B→C→D→A", Tour.Describe(square, [0, 1, 2, 3]));
    }
}
