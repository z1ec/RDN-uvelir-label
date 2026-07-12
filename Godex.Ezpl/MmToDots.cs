namespace Godex.Ezpl;

// Пересчёт мм → точки принтера в одном месте, как договорились в правилах проекта.
// Соотношение зафиксировано под конкретные DPI термопринтеров Godex (не общая формула
// dpi/25.4, а ровно те значения, что указаны в PROJECT_ANALYSIS.md и подтверждаются
// примерами в самом EZPL-мануале).
public static class MmToDots
{
    public static int Convert(double mm, int dpi)
    {
        var dotsPerMm = dpi switch
        {
            203 => 8,
            300 => 12,
            _ => throw new ArgumentOutOfRangeException(nameof(dpi), dpi, "Поддерживаются только 203 и 300 DPI")
        };

        return (int)Math.Round(mm * dotsPerMm);
    }
}
