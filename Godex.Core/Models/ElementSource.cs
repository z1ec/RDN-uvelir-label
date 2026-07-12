namespace Godex.Core.Models;

public sealed class ElementSource
{
    // "constant" — берём Text как есть; "column" — подставляем значение из колонки Excel по имени.
    public required string Kind { get; init; }

    public string? Text { get; init; }

    public string? ColumnName { get; init; }
}
