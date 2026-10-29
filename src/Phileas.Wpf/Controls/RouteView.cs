using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Phileas.Core.Models;

namespace Phileas.Wpf.Controls;

/// <summary>
/// Draws the cities and the tour through them. While <see cref="IsActive"/> is true a stream of light dots
/// runs along the route and the start city pulses, so a running search never looks frozen.
/// </summary>
public sealed class RouteView : FrameworkElement
{
    private const double Padding = 30;
    private static readonly Typeface LabelFont = new("Segoe UI Semibold");
    private static readonly Brush CityFill = new SolidColorBrush(Color.FromRgb(0x0B, 0x10, 0x1C));
    private static readonly Brush GridDot = new SolidColorBrush(Color.FromArgb(0x22, 0xFF, 0xFF, 0xFF));
    private static readonly Brush LabelBrush = new SolidColorBrush(Color.FromRgb(0xCB, 0xD5, 0xE1));

    public static readonly DependencyProperty CitiesProperty = DependencyProperty.Register(
        nameof(Cities), typeof(IReadOnlyList<City>), typeof(RouteView),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty OrderProperty = DependencyProperty.Register(
        nameof(Order), typeof(int[]), typeof(RouteView),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty AccentProperty = DependencyProperty.Register(
        nameof(Accent), typeof(Brush), typeof(RouteView),
        new FrameworkPropertyMetadata(Brushes.DeepSkyBlue, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive), typeof(bool), typeof(RouteView),
        new FrameworkPropertyMetadata(false, (d, e) => ((RouteView)d).SetAnimation((bool)e.NewValue)));

    private double _phase;
    private bool _animating;

    public RouteView() => ClipToBounds = true;

    public IReadOnlyList<City>? Cities
    {
        get => (IReadOnlyList<City>?)GetValue(CitiesProperty);
        set => SetValue(CitiesProperty, value);
    }

    public int[]? Order
    {
        get => (int[]?)GetValue(OrderProperty);
        set => SetValue(OrderProperty, value);
    }

    public Brush Accent
    {
        get => (Brush)GetValue(AccentProperty);
        set => SetValue(AccentProperty, value);
    }

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    private void SetAnimation(bool on)
    {
        if (on == _animating)
        {
            return;
        }

        _animating = on;
        if (on)
        {
            CompositionTarget.Rendering += OnFrame;
        }
        else
        {
            CompositionTarget.Rendering -= OnFrame;
            InvalidateVisual();
        }
    }

    private void OnFrame(object? sender, EventArgs e)
    {
        _phase += 0.25;
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc)
    {
        var scale = Math.Min((ActualWidth - 2 * Padding) / CityGenerator.Width, (ActualHeight - 2 * Padding) / CityGenerator.Height);
        if (Cities is not { Count: > 0 } cities || scale <= 0)
        {
            return;
        }

        var left = (ActualWidth - CityGenerator.Width * scale) / 2;
        var top = (ActualHeight - CityGenerator.Height * scale) / 2;
        Point ToScreen(City city) => new(left + city.X * scale, top + city.Y * scale);

        for (var x = 0; x <= CityGenerator.Width; x += 100)
        {
            for (var y = 0; y <= CityGenerator.Height; y += 100)
            {
                dc.DrawEllipse(GridDot, null, new Point(left + x * scale, top + y * scale), 1.2, 1.2);
            }
        }

        if (Order is { Length: > 1 } order)
        {
            DrawRoute(dc, order.Select(i => ToScreen(cities[i])).ToArray());
        }

        var startCity = Order is { Length: > 0 } ? cities[Order[0]] : null;
        foreach (var city in cities)
        {
            DrawCity(dc, city, ToScreen(city), isStart: city == startCity);
        }
    }

    private void DrawRoute(DrawingContext dc, Point[] points)
    {
        var geometry = new StreamGeometry();
        using (var context = geometry.Open())
        {
            context.BeginFigure(points[0], false, true);
            context.PolyLineTo(points.Skip(1).ToArray(), true, true);
        }
        geometry.Freeze();

        dc.DrawGeometry(null, new Pen(Accent, 8) { LineJoin = PenLineJoin.Round, Brush = WithOpacity(Accent, 0.16) }, geometry);
        dc.DrawGeometry(null, new Pen(Accent, 2.2) { LineJoin = PenLineJoin.Round }, geometry);

        if (IsActive)
        {
            var flow = new Pen(Brushes.White, 2.4) { DashCap = PenLineCap.Round, DashStyle = new DashStyle([0.1, 5], -_phase) };
            dc.DrawGeometry(null, flow, geometry);
        }

        for (var i = 0; i < points.Length; i++)
        {
            DrawArrow(dc, points[i], points[(i + 1) % points.Length]);
        }
    }

    private void DrawArrow(DrawingContext dc, Point from, Point to)
    {
        var direction = to - from;
        if (direction.Length < 24)
        {
            return;
        }

        direction.Normalize();
        var middle = new Point((from.X + to.X) / 2, (from.Y + to.Y) / 2);
        var side = new Vector(-direction.Y, direction.X);

        var geometry = new StreamGeometry();
        using (var context = geometry.Open())
        {
            context.BeginFigure(middle + direction * 5, true, true);
            context.LineTo(middle - direction * 4 + side * 4, false, false);
            context.LineTo(middle - direction * 4 - side * 4, false, false);
        }
        geometry.Freeze();
        dc.DrawGeometry(Accent, null, geometry);
    }

    private void DrawCity(DrawingContext dc, City city, Point center, bool isStart)
    {
        if (isStart)
        {
            if (IsActive)
            {
                var pulse = _phase % 12 / 12;
                dc.DrawEllipse(null, new Pen(WithOpacity(Accent, 1 - pulse), 2), center, 9 + pulse * 14, 9 + pulse * 14);
            }
            dc.DrawEllipse(Accent, new Pen(Brushes.White, 1.5), center, 8, 8);
        }
        else
        {
            dc.DrawEllipse(CityFill, new Pen(Accent, 2), center, 5, 5);
        }

        var label = new FormattedText(
            city.Name, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, LabelFont, 11, LabelBrush,
            VisualTreeHelper.GetDpi(this).PixelsPerDip);
        dc.DrawText(label, new Point(center.X + 8, center.Y - 20));
    }

    private static Brush WithOpacity(Brush brush, double opacity) =>
        brush is SolidColorBrush solid
            ? new SolidColorBrush(Color.FromArgb((byte)(opacity * 255), solid.Color.R, solid.Color.G, solid.Color.B))
            : brush;
}
