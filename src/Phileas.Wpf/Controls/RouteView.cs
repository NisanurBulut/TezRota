using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Phileas.Core.Models;

namespace Phileas.Wpf.Controls;

/// <summary>
/// Draws the cities and the tour through them. While <see cref="IsActive"/> is true a stream of light dots
/// runs along the route and the start city pulses, so a running search never looks frozen.
/// Pick locations in a <see cref="Warehouse"/> are drawn between racks and the route follows the aisles.
/// Hovering a pick shows the walk to it from the previous pick: metres per segment and the longer cross aisle, dashed.
/// </summary>
public sealed class RouteView : FrameworkElement
{
    private const double Padding = 30;
    private static readonly Typeface LabelFont = new("Segoe UI Semibold");
    private static readonly Brush CityFill = new SolidColorBrush(Colors.White);
    private static readonly Brush GridDot = new SolidColorBrush(Color.FromArgb(0x99, 0xBA, 0xC8, 0xD3));
    private static readonly Brush LabelBrush = new SolidColorBrush(Color.FromRgb(0x2A, 0x37, 0x44));
    private static readonly Brush RackFill = new SolidColorBrush(Color.FromRgb(0xD5, 0xDE, 0xE8));
    private static readonly Brush MutedBrush = new SolidColorBrush(Color.FromRgb(0x6B, 0x78, 0x86));
    private static readonly Pen AlternativePen = new(MutedBrush, 2) { DashStyle = new DashStyle([3, 2.5], 0), LineJoin = PenLineJoin.Round };
    private static readonly Pen BoxBorder = new(new SolidColorBrush(Color.FromRgb(0xC9, 0xD3, 0xDD)), 1);

    public static readonly DependencyProperty CitiesProperty = DependencyProperty.Register(
        nameof(Cities), typeof(IReadOnlyList<City>), typeof(RouteView),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty OrderProperty = DependencyProperty.Register(
        nameof(Order), typeof(int[]), typeof(RouteView),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty AccentProperty = DependencyProperty.Register(
        nameof(Accent), typeof(Brush), typeof(RouteView),
        new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x4F, 0x7F, 0xD6)), FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive), typeof(bool), typeof(RouteView),
        new FrameworkPropertyMetadata(false, (d, e) => ((RouteView)d).SetAnimation((bool)e.NewValue)));

    private double _phase;
    private bool _animating;
    private double _left;
    private double _top;
    private double _scale;
    private int? _hovered;

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

    protected override void OnMouseMove(MouseEventArgs e)
    {
        var position = e.GetPosition(this);
        int? nearest = null;
        var best = 14.0;
        if (Cities is { Count: > 0 } cities && cities[0].InWarehouse && _scale > 0)
        {
            for (var i = 0; i < cities.Count; i++)
            {
                var distance = (Screen(cities[i].X, cities[i].Y) - position).Length;
                if (distance < best)
                {
                    best = distance;
                    nearest = i;
                }
            }
        }
        SetHovered(nearest);
    }

    protected override void OnMouseLeave(MouseEventArgs e) => SetHovered(null);

    private void SetHovered(int? index)
    {
        if (index != _hovered)
        {
            _hovered = index;
            Cursor = index is null ? null : Cursors.Hand;
            InvalidateVisual();
        }
    }

    private Point Screen(double x, double y) => new(_left + x * _scale, _top + y * _scale);

    protected override void OnRender(DrawingContext dc)
    {
        // A transparent background makes the whole area receive mouse moves, not only the drawn lines.
        dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

        var scale = Math.Min((ActualWidth - 2 * Padding) / CityGenerator.Width, (ActualHeight - 2 * Padding) / CityGenerator.Height);
        if (Cities is not { Count: > 0 } cities || scale <= 0)
        {
            return;
        }

        var left = (ActualWidth - CityGenerator.Width * scale) / 2;
        var top = (ActualHeight - CityGenerator.Height * scale) / 2;
        (_left, _top, _scale) = (left, top, scale);
        Point ToScreen(City city) => new(left + city.X * scale, top + city.Y * scale);

        for (var x = 0; x <= CityGenerator.Width; x += 100)
        {
            for (var y = 0; y <= CityGenerator.Height; y += 100)
            {
                dc.DrawEllipse(GridDot, null, new Point(left + x * scale, top + y * scale), 1.2, 1.2);
            }
        }

        var inWarehouse = cities[0].InWarehouse;
        if (inWarehouse)
        {
            foreach (var x in Warehouse.Aisles.SelectMany(aisle => new[] { aisle - 15 - Warehouse.RackWidth, aisle + 15 }))
            {
                var rack = new Rect(left + x * scale, top + (Warehouse.Top + 30) * scale,
                    Warehouse.RackWidth * scale, (Warehouse.Bottom - Warehouse.Top - 60) * scale);
                dc.DrawRoundedRectangle(RackFill, null, rack, 3, 3);
            }
        }

        if (Order is { Length: > 1 } order)
        {
            var route = inWarehouse
                ? order.SelectMany((c, k) => Warehouse.Walk(cities[c], cities[order[(k + 1) % order.Length]]))
                    .Select(p => new Point(left + p.X * scale, top + p.Y * scale))
                : order.Select(i => ToScreen(cities[i]));
            DrawRoute(dc, route.ToArray());
        }

        var leg = inWarehouse ? HoveredLeg(cities) : null;
        if (leg is { } walk)
        {
            DrawLeg(dc, walk.From, walk.To);
        }

        var startCity = Order is { Length: > 0 } ? cities[Order[0]] : null;
        foreach (var city in cities)
        {
            DrawCity(dc, city, ToScreen(city), isStart: city == startCity);
        }

        if (leg is { } hovered)
        {
            DrawLegInfo(dc, hovered.From, hovered.To);
        }
        else if (inWarehouse && Order is { Length: > 1 })
        {
            dc.DrawText(Text("Hover a pick to see how the picker walks there", 11, MutedBrush), Screen(60, 552));
        }
    }

    private (City From, City To)? HoveredLeg(IReadOnlyList<City> cities)
    {
        if (_hovered is not { } index || index >= cities.Count || Order is not { Length: > 1 } order)
        {
            return null;
        }

        var k = Array.IndexOf(order, index);
        return k < 0 ? null : (cities[order[(k - 1 + order.Length) % order.Length]], cities[index]);
    }

    private void DrawLeg(DrawingContext dc, City from, City to)
    {
        var chosen = Warehouse.ShorterCrossAisle(from, to);
        if (from.X != to.X)
        {
            DrawPolyline(dc, LegPoints(from, Warehouse.Walk(from, to, Warehouse.OtherCrossAisle(chosen))), AlternativePen);
        }

        var points = LegPoints(from, Warehouse.Walk(from, to, chosen));
        DrawPolyline(dc, points, new Pen(Accent, 5) { LineJoin = PenLineJoin.Round, StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round });

        for (var i = 0; i + 1 < points.Length; i++)
        {
            var units = (points[i + 1] - points[i]).Length / _scale;
            if (units >= 1)
            {
                var middle = new Point((points[i].X + points[i + 1].X) / 2, (points[i].Y + points[i + 1].Y) / 2);
                DrawPill(dc, Text(Metres(units * Warehouse.MetresPerUnit), 11, Accent), middle);
            }
        }
    }

    private void DrawLegInfo(DrawingContext dc, City from, City to)
    {
        var chosen = Warehouse.ShorterCrossAisle(from, to);
        var metres = Metres(Warehouse.WalkDistance(from, to) * 1000);
        var title = from.X == to.X
            ? $"{from.Name} → {to.Name}: {metres} along the aisle"
            : $"{from.Name} → {to.Name}: {metres} via the {AisleName(chosen)} cross aisle";
        var lines = new List<FormattedText> { Text(title, 12, LabelBrush) };
        if (from.X != to.X)
        {
            var other = Warehouse.OtherCrossAisle(chosen);
            lines.Add(Text($"Via the {AisleName(other)}: {Metres(Warehouse.WalkDistance(from, to, other) * 1000)} (dashed)", 11, MutedBrush));
        }

        var width = lines.Max(l => l.Width) + 20;
        var height = lines.Sum(l => l.Height) + 14;
        var anchor = Screen(to.X, to.Y);
        var x = Math.Clamp(anchor.X + 16, 4, Math.Max(4, ActualWidth - width - 4));
        var y = Math.Clamp(anchor.Y + 16, 4, Math.Max(4, ActualHeight - height - 4));

        dc.DrawRoundedRectangle(CityFill, BoxBorder, new Rect(x, y, width, height), 6, 6);
        y += 7;
        foreach (var line in lines)
        {
            dc.DrawText(line, new Point(x + 10, y));
            y += line.Height;
        }
    }

    private Point[] LegPoints(City from, IEnumerable<(double X, double Y)> walk) =>
        [Screen(from.X, from.Y), .. walk.Select(p => Screen(p.X, p.Y))];

    private static void DrawPolyline(DrawingContext dc, Point[] points, Pen pen)
    {
        var geometry = new StreamGeometry();
        using (var context = geometry.Open())
        {
            context.BeginFigure(points[0], false, false);
            context.PolyLineTo(points.Skip(1).ToArray(), true, true);
        }
        geometry.Freeze();
        dc.DrawGeometry(null, pen, geometry);
    }

    private static void DrawPill(DrawingContext dc, FormattedText text, Point center)
    {
        var box = new Rect(center.X - text.Width / 2 - 5, center.Y - text.Height / 2 - 1, text.Width + 10, text.Height + 2);
        dc.DrawRoundedRectangle(CityFill, BoxBorder, box, 8, 8);
        dc.DrawText(text, new Point(box.X + 5, box.Y + 1));
    }

    private FormattedText Text(string text, double size, Brush brush) =>
        new(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, LabelFont, size, brush, VisualTreeHelper.GetDpi(this).PixelsPerDip);

    private static string Metres(double metres) => metres.ToString("0.#", CultureInfo.InvariantCulture) + " m";

    private static string AisleName(double crossAisle) => crossAisle == Warehouse.Top ? "top" : "bottom";

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
            dc.DrawEllipse(Accent, new Pen(Brushes.White, 2), center, 8, 8);
        }
        else
        {
            dc.DrawEllipse(CityFill, new Pen(Accent, 2), center, 5, 5);
        }

        dc.DrawText(Text(city.Name, 11, LabelBrush), new Point(center.X + 8, center.Y - 20));
    }

    private static Brush WithOpacity(Brush brush, double opacity) =>
        brush is SolidColorBrush solid
            ? new SolidColorBrush(Color.FromArgb((byte)(opacity * 255), solid.Color.R, solid.Color.G, solid.Color.B))
            : brush;
}
