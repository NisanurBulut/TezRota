using System.Globalization;

namespace Phileas.Wpf.ViewModels;

internal static class Display
{
    public static string Distance(double km) => string.Create(CultureInfo.InvariantCulture, $"{km:N0} km");

    public static string TravelTime(TimeSpan time) =>
        time.TotalHours >= 1 ? $"{(int)time.TotalHours} h {time.Minutes:00} min" : $"{time.Minutes} min {time.Seconds:00} s";

    public static string SolveTime(TimeSpan time) =>
        time.TotalSeconds >= 1
            ? string.Create(CultureInfo.InvariantCulture, $"{time.TotalSeconds:F2} s")
            : string.Create(CultureInfo.InvariantCulture, $"{time.TotalMilliseconds:F0} ms");

    public static string Count(long value) => value.ToString("N0", CultureInfo.InvariantCulture);
}
