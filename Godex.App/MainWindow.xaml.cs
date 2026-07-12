using Godex.App.ViewModels;
using Wpf.Ui.Controls;

namespace Godex.App;

public partial class MainWindow : FluentWindow
{
    // Единственное, что разрешено делать в code-behind по правилам проекта, —
    // это то, без чего WPF не обойдётся: InitializeComponent() и установка DataContext.
    // Вся остальная логика живёт во ViewModel.
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
