using System.Text.Json;

namespace Godex.Core.Models;

public static class LabelTemplateSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static LabelTemplate Load(string path)
    {
        var json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<LabelTemplate>(json, Options)
            ?? throw new InvalidOperationException($"Не удалось прочитать шаблон из файла «{path}».");
    }

    public static void Save(LabelTemplate template, string path)
    {
        var json = JsonSerializer.Serialize(template, Options);
        File.WriteAllText(path, json);
    }
}
