namespace Godex.Core.Models;

// Данные, загруженные из одного листа Excel: список колонок (заголовков)
// и список строк. Не знает про Excel/ClosedXML — с тем же успехом источником
// мог бы быть CSV или база данных, ничего в этой модели от этого не изменится.
public sealed class DataSource
{
    public required IReadOnlyList<string> Columns { get; init; }

    public required IReadOnlyList<DataSourceRow> Rows { get; init; }
}
