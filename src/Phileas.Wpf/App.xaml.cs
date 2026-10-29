using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Phileas.Wpf.Infrastructure;
using Phileas.Wpf.Services;
using Phileas.Wpf.ViewModels;
using Phileas.Wpf.Views;

namespace Phileas.Wpf;

public partial class App : Application
{
    private readonly IHost _host = CreateHost();

    private static IHost CreateHost()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings { ContentRootPath = AppContext.BaseDirectory });

        builder.Services.Configure<OptimizationDefaults>(builder.Configuration.GetSection("Defaults"));
        builder.Services.AddSingleton<IOptimizationService, OptimizationService>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainWindow>();

        return builder.Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _host.Services.GetRequiredService<MainWindow>().Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host.Dispose();
        base.OnExit(e);
    }
}
