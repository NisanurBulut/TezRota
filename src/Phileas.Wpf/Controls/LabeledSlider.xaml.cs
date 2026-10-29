using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Phileas.Wpf.Controls;

/// <summary>A caption, the current value and a slider in one row; view-only code, no business logic.</summary>
public partial class LabeledSlider : UserControl
{
    public static readonly DependencyProperty HeaderProperty = Register(nameof(Header), "");
    public static readonly DependencyProperty ValueProperty = Register(nameof(Value), 0.0, OnValueChanged, bindsTwoWay: true);
    public static readonly DependencyProperty MinimumProperty = Register(nameof(Minimum), 0.0);
    public static readonly DependencyProperty MaximumProperty = Register(nameof(Maximum), 100.0);
    public static readonly DependencyProperty StepProperty = Register(nameof(Step), 1.0);
    public static readonly DependencyProperty FormatProperty = Register(nameof(Format), "0", OnValueChanged);
    public static readonly DependencyProperty AccentProperty = Register<Brush>(nameof(Accent), Brushes.DeepSkyBlue);

    public LabeledSlider() => InitializeComponent();

    public string Header { get => (string)GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }
    public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public double Minimum { get => (double)GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public double Maximum { get => (double)GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    public double Step { get => (double)GetValue(StepProperty); set => SetValue(StepProperty, value); }

    /// <summary>.NET number format of the value text, e.g. "0.000" or "0 km/h".</summary>
    public string Format { get => (string)GetValue(FormatProperty); set => SetValue(FormatProperty, value); }

    public Brush Accent { get => (Brush)GetValue(AccentProperty); set => SetValue(AccentProperty, value); }

    private static DependencyProperty Register<T>(string name, T defaultValue, PropertyChangedCallback? changed = null, bool bindsTwoWay = false) =>
        DependencyProperty.Register(name, typeof(T), typeof(LabeledSlider),
            new FrameworkPropertyMetadata(defaultValue, bindsTwoWay ? FrameworkPropertyMetadataOptions.BindsTwoWayByDefault : FrameworkPropertyMetadataOptions.None, changed));

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((LabeledSlider)d).UpdateValueText();

    private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => UpdateValueText();

    private void UpdateValueText() => ValueText.Text = Value.ToString(Format, CultureInfo.InvariantCulture);
}
