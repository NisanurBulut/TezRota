using System.Windows;
using Phileas.Wpf.ViewModels;

namespace Phileas.Wpf.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
