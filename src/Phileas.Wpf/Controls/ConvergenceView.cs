using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Phileas.Core.Models;

namespace Phileas.Wpf.Controls;

/// <summary>
/// Line chart of the best distance over the share of iterations done, for both methods.
/// The lines grow while the search is replayed (see the Replayed* properties).
/// </summary>
public sealed class ConvergenceView : FrameworkElement
{
    private const double LeftMargin = 56;
    private const double PlotMargin = 12;
    private const double BottomMargin = 26;
    private static readonly Typeface AxisFont = new("Segoe UI");
    private static readonly Brush AxisBrush = new SolidColorBrush(Color.FromRgb(0x56, 0x65, 0x74));
    private static readonly Pen GridPen = new(new SolidColorBrush(Color.FromArgb(0x24, 0x13, 0x1A, 0x22)), 1);

    public static readonly DependencyProperty AnnealingResultProperty = Register<SolveResult?>(nameof(AnnealingResult), null);
    public static readonly DependencyProperty AnnealingReplayedProperty = Register(nameof(AnnealingReplayed), 0L);
    public static readonly DependencyProperty AnnealingBrushProperty = Register<Brush>(nameof(AnnealingBrush), new SolidColorBrush(Color.FromRgb(0x4F, 0x7F, 0xD6)));
    public static readonly DependencyProperty TabuResultProperty = Register<SolveResult?>(nameof(TabuResult), null);
    public static readonly DependencyProperty TabuReplayedProperty = Register(nameof(TabuReplayed), 0L);
    public static readonly DependencyProperty TabuBrushProperty = Register<Brush>(nameof(TabuBrush), new SolidColorBrush(Color.FromRgb(0xC9, 0x7C, 0x5D)));

    public SolveResult? AnnealingResult { get => (SolveResult?)GetValue(AnnealingResultProperty); set => SetValue(AnnealingResultProperty, value); }
    public long AnnealingReplayed { get => (long)GetValue(AnnealingReplayedProperty); set => SetValue(AnnealingReplayedProperty, value); }
    public Brush AnnealingBrush { get => (Brush)GetValue(AnnealingBrushProperty); set => SetValue(AnnealingBrushProperty, value); }
    public SolveResult? TabuResult { get => (SolveResult?)GetValue(TabuResultProperty); set => SetValue(TabuResultProperty, value); }
    public long TabuReplayed { get => (long)GetValue(TabuReplayedProperty); set => SetValue(TabuReplayedProperty, value); }
    public Brush TabuBrush { get => (Brush)GetValue(TabuBrushProperty); set => SetValue(TabuBrushProperty, value); }

    public ConvergenceView() => ClipToBounds = true;

    private static DependencyProperty Register<T>(string name, T defaultValue) =>
        DependencyProperty.Register(name, typeof(T), typeof(ConvergenceView),
            new FrameworkPropertyMetadata(defaultValue, FrameworkPropertyMetadataOptions.AffectsRender));

    protected override void OnRender(DrawingContext dc)
    {
        var plot = new Rect(LeftMargin, PlotMargin, ActualWidth - LeftMargin - PlotMargin, ActualHeight - PlotMargin - BottomMargin);
        if (plot.Width <= 0 || plot.Height <= 0)
        {
            return;
        }

        SolveResult?[] results = [AnnealingResult, TabuResult];
        var finished = results.OfType<SolveResult>().ToList();
        if (finished.Count == 0)
        {
            DrawText(dc, "Run a search to see how the distance shrinks", new Point(plot.Left, plot.Top + plot.Height / 2 - 8), 12);
            return;
        }

        var minDistance = finished.Min(r => r.Distance) * 0.95;
        // Zoom in on the interesting part: the random start tours are far longer than the results and would flatten the curves.
        var maxDistance = Math.Min(finished.Max(r => r.StartDistance), minDistance * 2);
        double Y(double distance) => Math.Max(plot.Top, plot.Bottom - (distance - minDistance) / (maxDistance - minDistance) * plot.Height);

        for (var i = 0; i <= 3; i++)
        {
            var distance = minDistance + (maxDistance - minDistance) * i / 3;
            dc.DrawLine(GridPen, new Point(plot.Left, Y(distance)), new Point(plot.Right, Y(distance)));
            DrawText(dc, $"{distance:N0}", new Point(4, Y(distance) - 8), 11);
        }
        DrawText(dc, "0 %", new Point(plot.Left, plot.Bottom + 6), 11);
        DrawText(dc, "iterations done", new Point(plot.Left + plot.Width / 2 - 36, plot.Bottom + 6), 11);
        DrawText(dc, "100 %", new Point(plot.Right - 28, plot.Bottom + 6), 11);

        DrawSeries(dc, plot, AnnealingResult, AnnealingReplayed, AnnealingBrush, Y);
        DrawSeries(dc, plot, TabuResult, TabuReplayed, TabuBrush, Y);
    }

    private static void DrawSeries(DrawingContext dc, Rect plot, SolveResult? result, long replayed, Brush brush, Func<double, double> y)
    {
        if (result is null)
        {
            return;
        }

        double X(long iteration) => plot.Left + (double)iteration / result.Iterations * plot.Width;

        var shown = result.Snapshots.Where(s => s.Iteration <= replayed).ToList();
        if (shown.Count == 0)
        {
            return;
        }

        // Step line: the best distance stays the same until the next improvement.
        var geometry = new StreamGeometry();
        using (var context = geometry.Open())
        {
            context.BeginFigure(new Point(X(shown[0].Iteration), y(shown[0].Distance)), false, false);
            for (var i = 1; i < shown.Count; i++)
            {
                context.LineTo(new Point(X(shown[i].Iteration), y(shown[i - 1].Distance)), true, true);
                context.LineTo(new Point(X(shown[i].Iteration), y(shown[i].Distance)), true, true);
            }
            context.LineTo(new Point(X(Math.Min(replayed, result.Iterations)), y(shown[^1].Distance)), true, true);
        }
        geometry.Freeze();

        dc.DrawGeometry(null, new Pen(brush, 2.4) { LineJoin = PenLineJoin.Round }, geometry);
        dc.DrawEllipse(brush, null, new Point(X(Math.Min(replayed, result.Iterations)), y(shown[^1].Distance)), 4, 4);
    }

    private void DrawText(DrawingContext dc, string text, Point origin, double size) =>
        dc.DrawText(
            new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, AxisFont, size, AxisBrush,
                VisualTreeHelper.GetDpi(this).PixelsPerDip),
            origin);
}
