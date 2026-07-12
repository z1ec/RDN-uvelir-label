namespace Godex.Core.Models;

public sealed class LabelTemplate
{
    public int Version { get; init; } = 1;

    public required LabelSize Label { get; init; }

    public required PrintSettings Print { get; init; }

    public required IReadOnlyList<LabelElement> Elements { get; init; }
}
