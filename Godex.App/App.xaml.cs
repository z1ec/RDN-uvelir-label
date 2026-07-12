using System.IO;
using System.Windows;
using Godex.App.ViewModels;
using Godex.Excel;
using Godex.Ezpl;
using Godex.Printing;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Godex.App;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        // Логи пишем в %LocalAppData%\Godex\logs, чтобы не зависеть от того,
        // откуда запущен exe, и чтобы файлы не терялись при переустановке программы.
        var logsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Godex", "logs", "godex-.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug()
            .WriteTo.File(logsPath, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Serilog.ILogger регистрируем как готовый экземпляр (не через DI-конструктор),
        // потому что Log.Logger уже настроен и создан выше, в конструкторе App.
        services.AddSingleton(Log.Logger);

        services.AddSingleton<ExcelWorkbookReader>();
        services.AddSingleton<EzplLabelGenerator>();
        services.AddSingleton<UsbPrinterSender>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Log.Logger.Information("Приложение запускается");

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Logger.Information("Приложение завершает работу");
        Log.CloseAndFlush();

        base.OnExit(e);
    }
}
