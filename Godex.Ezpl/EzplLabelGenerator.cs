using System.Text;
using Godex.Core.Models;

namespace Godex.Ezpl;

public sealed class EzplLabelGenerator
{
    public string Generate(LabelTemplate template, DataSourceRow row)
    {
        var sb = new StringBuilder();
        var dpi = template.Label.Dpi;

        // ^Q — длина этикетки и зазор между этикетками, EZPL manual p.10
        sb.Append($"^Q{template.Label.HeightMm:0.##},{template.Label.GapMm:0.##}\r\n");
        // ^W — ширина этикетки, EZPL manual p.12
        sb.Append($"^W{template.Label.WidthMm:0.##}\r\n");
        // ^H — темнота печати, 00-19, EZPL manual p.5
        sb.Append($"^H{template.Print.Darkness:00}\r\n");
        // ^S — скорость печати, 2-7 in/s, EZPL manual p.11
        sb.Append($"^S{template.Print.Speed}\r\n");
        // ^A — режим печати: D = термо, T = термотрансфер, EZPL manual p.2
        sb.Append($"^A{template.Print.Mode}\r\n");
        // Кодовая страница Windows-1251, чтобы кириллица не превращалась в кракозябры,
        // EZPL manual, ^XSET,CODEPAGE, p.14-15 (n=16 — WINDOWS 1251)
        sb.Append("^XSET,CODEPAGE,16\r\n");
        // ^L — начало описания этикетки, EZPL manual p.6
        sb.Append("^L\r\n");

        foreach (var element in template.Elements.Where(e => e.Type == "text"))
        {
            var text = ResolveText(element, row);

            var x = MmToDots.Convert(element.XMm, dpi);
            var y = MmToDots.Convert(element.YMm, dpi);

            // A<font>,x,y,x_mul,y_mul,gap,rotationInverse,data — текст, EZPL manual p.39.
            // Буква шрифта — часть имени команды (например "AD"), а не отдельный параметр.
            sb.Append($"A{element.Font},{x},{y},1,1,0,0,{text}\r\n");
        }

        // E — завершение описания этикетки и запуск печати, EZPL manual p.46
        sb.Append("E\r\n");

        return sb.ToString();
    }

    private static string ResolveText(LabelElement element, DataSourceRow row)
    {
        if (element.Source.Kind == "column" && element.Source.ColumnName is not null)
            return row.Values.GetValueOrDefault(element.Source.ColumnName, string.Empty);

        return element.Source.Text ?? string.Empty;
    }
}
