using ClosedXML.Excel;
using Godex.Core.Models;

namespace Godex.Excel;

public sealed class ExcelWorkbookReader
{
    public IReadOnlyList<ExcelSheetInfo> GetSheets(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);

        return workbook.Worksheets
            .Select(sheet => new ExcelSheetInfo
            {
                Name = sheet.Name,
                RowCount = sheet.RangeUsed()?.RowCount() ?? 0
            })
            .ToList();
    }

    // F1.3: первая строка листа всегда считается строкой заголовков — в MVP это не настраивается.
    public DataSource ReadSheet(string filePath, string sheetName)
    {
        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(sheetName);

        var usedRange = worksheet.RangeUsed()
            ?? throw new InvalidOperationException($"Лист «{sheetName}» пустой.");

        var rows = usedRange.RowsUsed().ToList();
        if (rows.Count == 0)
            throw new InvalidOperationException($"Лист «{sheetName}» не содержит данных.");

        var columns = rows[0].Cells()
            .Select(cell => cell.GetString().Trim())
            .ToList();

        var dataRows = new List<DataSourceRow>();
        for (var i = 1; i < rows.Count; i++)
        {
            var cells = rows[i].Cells().ToList();
            var values = new Dictionary<string, string>();

            for (var c = 0; c < columns.Count; c++)
                values[columns[c]] = cells[c].GetString();

            dataRows.Add(new DataSourceRow { Values = values });
        }

        return new DataSource { Columns = columns, Rows = dataRows };
    }
}
