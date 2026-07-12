namespace Godex.Core.Models;

public sealed class LabelElement
{
    public required string Id { get; init; }

    // Пока единственный поддерживаемый тип элемента — "text".
    // Штрихкод/QR/линии/прямоугольники добавятся вместе с полноценным редактором.
    public string Type { get; init; } = "text";

    public required double XMm { get; init; }

    public required double YMm { get; init; }

    // Буква bitmap-шрифта EZPL, A-H (команда At, EZPL manual p.39)
    public string Font { get; init; } = "D";

    public required ElementSource Source { get; init; }
}
