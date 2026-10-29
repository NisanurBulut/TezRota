using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Phileas.Core.Algorithms;
using Phileas.Core.Models;
using Phileas.Wpf.Infrastructure;
using Phileas.Wpf.Services;

namespace Phileas.Wpf.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly ILogger<MainViewModel> _logger;
    private CancellationTokenSource? _cancellation;
    private int[] _startOrder = [];

    public MainViewModel(IOptimizationService optimizer, IOptions<OptimizationDefaults> options, ILogger<MainViewModel> logger)
    {
        _logger = logger;

        var defaults = options.Value;
        Annealing = new AnnealingViewModel(optimizer, defaults.Annealing);
        Tabu = new TabuViewModel(optimizer, defaults.Tabu);
        CityCount = defaults.CityCount;
        SpeedKmPerHour = defaults.SpeedKmPerHour;
        AnimationSeconds = defaults.AnimationSeconds;

        GenerateCities();
    }

    public AnnealingViewModel Annealing { get; }
    public TabuViewModel Tabu { get; }
    public ResultViewModel Results { get; } = new();

    [ObservableProperty]
    public partial IReadOnlyList<City> Cities { get; private set; } = [];

    [ObservableProperty]
    public partial int CityCount { get; set; }

    [ObservableProperty]
    public partial double SpeedKmPerHour { get; set; }

    [ObservableProperty]
    public partial double AnimationSeconds { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GenerateCitiesCommand), nameof(CompareCommand), nameof(RunAnnealingCommand), nameof(RunTabuCommand), nameof(CancelCommand))]
    public partial bool IsRunning { get; private set; }

    partial void OnSpeedKmPerHourChanged(double value)
    {
        Annealing.SpeedKmPerHour = value;
        Tabu.SpeedKmPerHour = value;
        Results.Update(FinishedResults(), value);
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private void GenerateCities()
    {
        Cities = CityGenerator.Create(CityCount);
        _startOrder = Tour.RandomOrder(Cities.Count, Random.Shared);

        Annealing.Reset(Cities, _startOrder);
        Tabu.Reset(Cities, _startOrder);
        Annealing.SpeedKmPerHour = Tabu.SpeedKmPerHour = SpeedKmPerHour;
        Results.Update([], SpeedKmPerHour);
        _logger.LogInformation("Generated {Count} cities", Cities.Count);
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private Task CompareAsync() => RunAsync(Annealing, Tabu);

    [RelayCommand(CanExecute = nameof(CanStart))]
    private Task RunAnnealingAsync() => RunAsync(Annealing);

    [RelayCommand(CanExecute = nameof(CanStart))]
    private Task RunTabuAsync() => RunAsync(Tabu);

    [RelayCommand(CanExecute = nameof(IsRunning))]
    private void Cancel() => _cancellation?.Cancel();

    private bool CanStart() => !IsRunning;

    private async Task RunAsync(params AlgorithmViewModel[] algorithms)
    {
        using var cancellation = _cancellation = new CancellationTokenSource();
        IsRunning = true;
        try
        {
            var replay = TimeSpan.FromSeconds(AnimationSeconds);
            await Task.WhenAll(algorithms.Select(a => a.RunAsync(_startOrder, replay, cancellation.Token)));
            Results.Update(FinishedResults(), SpeedKmPerHour);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Run cancelled");
        }
        finally
        {
            IsRunning = false;
        }
    }

    private IEnumerable<SolveResult> FinishedResults() =>
        new AlgorithmViewModel[] { Annealing, Tabu }
            .Where(a => a.State == AlgorithmState.Done && a.Result is not null)
            .Select(a => a.Result!);
}
