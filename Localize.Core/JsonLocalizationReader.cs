namespace Localize;

/// <summary>
/// Reads a JSON localization file. Three shapes are accepted:
/// a flat dictionary, a nested object flattened with dots, and a language-pack document
/// (<c>cultureCode</c> plus <c>localizedStrings</c>), which is the file Inferno exports.
/// </summary>
public static class JsonLocalizationReader
{
    public static bool TryRead(
        string json,
        out string? cultureCode,
        out IReadOnlyDictionary<string, string> strings,
        out string? error)
    {
        cultureCode = null;
        strings = new Dictionary<string, string>(StringComparer.Ordinal);
        error = null;

        if (string.IsNullOrWhiteSpace(json))
        {
            error = "The file is empty.";
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                error = "The JSON root must be an object.";
                return false;
            }

            var root = document.RootElement;
            if (TryGet(root, "LocalizedStrings", out var pack) && pack.ValueKind == JsonValueKind.Object)
            {
                cultureCode = ReadCulture(root);
                strings = Flatten(pack, prefix: null);
                return true;
            }

            if (TryGet(root, "strings", out var nested) && nested.ValueKind == JsonValueKind.Object)
            {
                cultureCode = ReadCulture(root);
                strings = Flatten(nested, prefix: null);
                return true;
            }

            strings = Flatten(root, prefix: null);
            return true;
        }
        catch (JsonException ex)
        {
            error = $"That file is not valid JSON. {ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// Culture for a file. A parent folder named <c>fr-FR</c> wins, then the file name
    /// (<c>fr-FR.json</c>, <c>default.json</c>, <c>invariant.json</c>). Returns null when the
    /// path is not a localization file, so a stray <c>Home.json</c> is ignored.
    /// </summary>
    public static string? CultureFromPath(string path)
    {
        string file = Path.GetFileNameWithoutExtension(path);
        string? directory = Path.GetDirectoryName(path);
        string parent = string.IsNullOrEmpty(directory) ? "" : Path.GetFileName(directory);

        if (TryCultureToken(parent, out string parentCulture))
        {
            return parentCulture;
        }

        if (TryCultureToken(file, out string fileCulture))
        {
            return fileCulture;
        }

        return null;
    }

    public static Dictionary<string, string> Flatten(JsonElement element, string? prefix)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        if (element.ValueKind != JsonValueKind.Object)
        {
            return result;
        }

        foreach (var property in element.EnumerateObject())
        {
            string key = prefix is null ? property.Name : $"{prefix}.{property.Name}";
            switch (property.Value.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var nested in Flatten(property.Value, key))
                    {
                        result[nested.Key] = nested.Value;
                    }

                    break;
                case JsonValueKind.String:
                    result[key] = property.Value.GetString() ?? "";
                    break;
                case JsonValueKind.Null:
                    break;
                default:
                    result[key] = property.Value.ToString();
                    break;
            }
        }

        return result;
    }

    private static string? ReadCulture(JsonElement root)
    {
        if (TryGet(root, "CultureCode", out var culture) && culture.ValueKind == JsonValueKind.String)
        {
            return culture.GetString();
        }

        if (TryGet(root, "culture", out var alt) && alt.ValueKind == JsonValueKind.String)
        {
            return alt.GetString();
        }

        return null;
    }

    private static bool TryGet(JsonElement element, string name, out JsonElement value)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (property.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private static bool TryCultureToken(string? token, out string culture)
    {
        culture = CultureCodes.Invariant;
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        if (token.Equals("invariant", StringComparison.OrdinalIgnoreCase)
            || token.Equals("default", StringComparison.OrdinalIgnoreCase)
            || token.Equals("_invariant", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!CultureCodes.IsKnown(token))
        {
            return false;
        }

        culture = CultureCodes.Normalize(token);
        return true;
    }
}
