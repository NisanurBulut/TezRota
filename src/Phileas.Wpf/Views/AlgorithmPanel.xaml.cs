using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Phileas.Wpf.Views;

public partial class AlgorithmPanel : UserControl
{
    public static readonly DependencyProperty AccentProperty =
        DependencyProperty.Register(nameof(Accent), typeof(Brush), typeof(AlgorithmPanel), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0x4F, 0x7F, 0xD6))));

    public AlgorithmPanel() => InitializeComponent();

    public Brush Accent
    {
        get => (Brush)GetValue(AccentProperty);
        set => SetValue(AccentProperty, value);
    }
}
