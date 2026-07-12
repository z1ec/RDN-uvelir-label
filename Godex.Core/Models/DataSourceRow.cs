namespace Godex.Core.Models;

public sealed class DataSourceRow
{
    // Ключ — имя колонки (как в DataSource.Columns), значение — текст ячейки.
    public required IReadOnlyDictionary<string, string> Values { get; init; }
}
