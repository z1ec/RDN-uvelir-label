namespace Godex.Core.Models;

public sealed class PrintSettings
{
    // 2-7 in/s, EZPL manual ^S, p.11
    public required int Speed { get; init; }

    // 00-19, EZPL manual ^H, p.5
    public required int Darkness { get; init; }

    // "D" = термо, "T" = термотрансфер, EZPL manual ^A, p.2
    public required string Mode { get; init; }
}
