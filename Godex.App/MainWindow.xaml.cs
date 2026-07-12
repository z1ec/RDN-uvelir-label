using System.Windows.Controls;
using Godex.App.ViewModels;
using Wpf.Ui.Controls;

namespace Godex.App;

public partial class MainWindow : FluentWindow
{
    // Единственное, что разрешено делать в code-behind по правилам проекта, —
    // это то, без чего WPF не обойдётся: InitializeComponent(), установка DataContext
    // и подписка на события UI-контролов, у которых нет команды-аналога.
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    // AutoGeneratingColumn — событие конкретно DataGrid, генерируется на каждую
    // колонку при автогенерации. Команды/биндинга для этого в WPF не существует,
    // поэтому логика (запрет редактирования всех колонок, кроме чекбокса "Выбрано")
    // остаётся здесь, а не во ViewModel.
    private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.PropertyName != "Выбрано")
            e.Column.IsReadOnly = true;
    }
}
