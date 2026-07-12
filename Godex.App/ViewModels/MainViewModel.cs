using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;

namespace Godex.App.ViewModels;

// ObservableObject из CommunityToolkit.Mvvm умеет уведомлять View об изменении свойств
// (реализует INotifyPropertyChanged за нас). Атрибут [ObservableProperty] на приватном поле
// автоматически генерирует публичное свойство Title/Greeting с этим уведомлением —
// писать вручную "public string Title { get => ...; set { ...; OnPropertyChanged(); } }" не нужно.
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Godex Label Printer";

    [ObservableProperty]
    private string _greeting = "Каркас проекта готов: DI, Serilog и WPF-UI подключены.";

    public MainViewModel(ILogger logger)
    {
        logger.Information("MainViewModel создан");
    }
}
