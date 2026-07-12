namespace Godex.Core.Models;

public sealed class LabelSize
{
    public required double WidthMm { get; init; }

    public required double HeightMm { get; init; }

    public required double GapMm { get; init; }

    public required int Dpi { get; init; }
}
