using System.Collections.ObjectModel;
using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Godex.Core.Models;
using Godex.Excel;
using Microsoft.Win32;
using Serilog;

namespace Godex.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ExcelWorkbookReader _excelReader;
    private readonly ILogger _logger;

    private string? _filePath;

    [ObservableProperty]
    private string _title = "Godex Label Printer";

    [ObservableProperty]
    private ObservableCollection<string> _sheets = new();

    [ObservableProperty]
    private string? _selectedSheet;

    [ObservableProperty]
    private DataTable? _previewTable;

    [ObservableProperty]
    private string _statusMessage = "Файл не загружен";

    [ObservableProperty]
    private string _rangeFromText = "1";

    [ObservableProperty]
    private string _rangeToText = "1";

    public MainViewModel(ExcelWorkbookReader excelReader, ILogger logger)
    {
        _excelReader = excelReader;
        _logger = logger;
    }

    [RelayCommand]
    private void OpenFile()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Excel файлы (*.xlsx)|*.xlsx",
            Title = "Выберите файл Excel"
        };

        if (dialog.ShowDialog() != true)
            return;

        _filePath = dialog.FileName;

        try
        {
            var sheetInfos = _excelReader.GetSheets(_filePath);
            Sheets = new ObservableCollection<string>(sheetInfos.Select(s => s.Name));

            if (Sheets.Count == 1)
                SelectedSheet = Sheets[0];
            else
                StatusMessage = $"Файл открыт, выберите лист ({Sheets.Count} доступно)";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Не удалось открыть файл {FilePath}", _filePath);
            StatusMessage = $"Не удалось открыть файл: {ex.Message}";
        }
    }

    // CommunityToolkit.Mvvm генерирует этот хук автоматически для каждого [ObservableProperty] —
    // вызывается сразу после того, как SelectedSheet поменялся (в том числе из XAML-биндинга ComboBox).
    partial void OnSelectedSheetChanged(string? value)
    {
        if (value is null || _filePath is null)
            return;

        try
        {
            var dataSource = _excelReader.ReadSheet(_filePath, value);
            PreviewTable = BuildPreviewTable(dataSource);
            StatusMessage = $"Загружено строк: {dataSource.Rows.Count}";
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Не удалось прочитать лист {SheetName}", value);
            StatusMessage = $"Не удалось прочитать лист: {ex.Message}";
        }
    }

    // DataSource (наша чистая модель) превращаем в System.Data.DataTable здесь же, во ViewModel,
    // потому что это чисто вопрос отображения в WPF DataGrid — самому DataSource про это знать не нужно.
    // Колонка "Выбрано" добавляется первой и вручную не биндится в XAML: DataGrid с AutoGenerateColumns
    // сам генерирует для bool-колонки чекбокс.
    private static DataTable BuildPreviewTable(DataSource dataSource)
    {
        var table = new DataTable();
        table.Columns.Add("Выбрано", typeof(bool));

        foreach (var column in dataSource.Columns)
            table.Columns.Add(column, typeof(string));

        foreach (var row in dataSource.Rows)
        {
            var dataRow = table.NewRow();
            dataRow["Выбрано"] = false;

            foreach (var column in dataSource.Columns)
                dataRow[column] = row.Values[column];

            table.Rows.Add(dataRow);
        }

        return table;
    }

    [RelayCommand]
    private void SelectAll() => SetSelection(true);

    [RelayCommand]
    private void ClearSelection() => SetSelection(false);

    private void SetSelection(bool value)
    {
        if (PreviewTable is null)
            return;

        foreach (DataRow row in PreviewTable.Rows)
            row["Выбрано"] = value;
    }

    [RelayCommand]
    private void SelectRange()
    {
        if (PreviewTable is null)
            return;

        // Парсим строки вручную, а не биндим TextBox напрямую на int — так ошибка ввода
        // (например, буквы вместо цифр) обрабатывается явно и понятно, а не тонет
        // где-то в недрах механизма конвертации биндинга WPF.
        if (!int.TryParse(RangeFromText, out var from) || !int.TryParse(RangeToText, out var to))
        {
            StatusMessage = "Диапазон строк должен быть числом";
            return;
        }

        from = Math.Max(1, from);
        to = Math.Min(PreviewTable.Rows.Count, to);

        if (from > to)
        {
            StatusMessage = "Начало диапазона больше конца";
            return;
        }

        for (var i = from; i <= to; i++)
            PreviewTable.Rows[i - 1]["Выбрано"] = true;

        StatusMessage = $"Выбрано строк: {to - from + 1}";
    }
}
