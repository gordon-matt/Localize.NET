using System.Globalization;
using Localize;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Localize.Demo.Components.Pages;

public partial class Home
{
    private static readonly string[] Keys =
    [
        "Demo/Home.Title",
        "Demo/Home.Body",
        "Demo/Home.Greeting",
        "Demo/SameCulture",
        "Demo/Fallback.Parent",
        "Demo/Only.Database",
        "Demo/Only.Pack",
        "Demo/Only.Resx",
        "Demo/Only.Json",
        "Demo.Nested.Note",
    ];

    [Inject]
    public IStringLocalizer Localizer { get; set; } = default!;

    [Inject]
    public IEnumerable<ILocalizationSource> Sources { get; set; } = default!;

    [Inject]
    public ILocalizationScope Scope { get; set; } = default!;

    [Inject]
    public IOptions<LocalizeOptions> Options { get; set; } = default!;

    private IReadOnlyList<ILocalizationSource> OrderedSources { get; set; } = [];

    private IReadOnlyList<DemoRow> Rows { get; set; } = [];

    private string Culture { get; set; } = "";

    private string ChainLabel { get; set; } = "";

    protected override void OnInitialized()
    {
        Culture = CultureInfo.CurrentUICulture.Name;
        OrderedSources = Sources.OrderBy(source => source.Priority).ThenBy(source => source.Name, StringComparer.Ordinal).ToArray();

        var chain = CultureCodes.FallbackChain(Culture, Options.Value.UseParentCultureFallback);
        ChainLabel = string.Join(" → ", chain.Select(CultureLabel));
        Rows = Build(chain);
    }

    private List<DemoRow> Build(IReadOnlyList<string> chain)
    {
        string scopeId = CultureCodes.NormalizeScope(Scope.GetScopeId());
        var loaded = new Dictionary<string, Dictionary<string, IReadOnlyDictionary<string, string>>>(StringComparer.Ordinal);
        foreach (var source in OrderedSources)
        {
            var perCulture = new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.Ordinal);
            foreach (string step in chain)
            {
                perCulture[step] = source.GetAll(scopeId, step);
            }

            loaded[source.Name] = perCulture;
        }

        var rows = new List<DemoRow>(Keys.Length);
        foreach (string key in Keys)
        {
            Contribution? winner = null;
            for (int index = chain.Count - 1; index >= 0; index--)
            {
                string step = chain[index];
                foreach (var source in OrderedSources)
                {
                    if (loaded[source.Name][step].TryGetValue(key, out string? text))
                    {
                        winner = new Contribution(source.Name, source.Priority, step, text);
                    }
                }
            }

            var present = new List<Contribution>();
            var cells = new List<SourceCell>(OrderedSources.Count);
            foreach (var source in OrderedSources)
            {
                string? text = null;
                string? from = null;
                foreach (string step in chain)
                {
                    if (loaded[source.Name][step].TryGetValue(key, out string? found))
                    {
                        text = found;
                        from = step;
                        break;
                    }
                }

                if (text is not null && from is not null)
                {
                    present.Add(new Contribution(source.Name, source.Priority, from, text));
                }

                bool wins = winner is not null && text is not null && winner.Source == source.Name && winner.Culture == from;
                cells.Add(new SourceCell(text, from is null ? null : CultureLabel(from), wins));
            }

            rows.Add(new DemoRow(key, Explain(Culture, chain, winner, present), cells));
        }

        return rows;
    }

    private static string Explain(string request, IReadOnlyList<string> chain, Contribution? winner, IReadOnlyList<Contribution> present)
    {
        if (winner is null)
        {
            return "Every source is missing this key, so the key itself is shown.";
        }

        string requestLabel = CultureLabel(request);
        string winnerCulture = CultureLabel(winner.Culture);
        string winnerName = SourceLabel(winner.Source);
        var others = present.Where(item => item.Source != winner.Source).ToArray();
        if (others.Length == 0)
        {
            return winner.Culture == request
                ? OnlySource(winnerName)
                : $"No {requestLabel} text for this key, so the {winnerCulture} {winnerName} is used.";
        }

        var sameCulture = others
            .Where(item => item.Culture == winner.Culture)
            .OrderByDescending(item => item.Priority)
            .FirstOrDefault();
        if (sameCulture is not null)
        {
            string place = winner.Culture == request
                ? requestLabel
                : $"the {winnerCulture} culture. No {requestLabel} text for this key";
            return $"{Lead(winnerName)} ({winner.Priority}) wins over {SourceLabel(sameCulture.Source)} ({sameCulture.Priority}) in {place}.";
        }

        var next = others
            .OrderBy(item => Rank(chain, item.Culture))
            .ThenByDescending(item => item.Priority)
            .First();
        return $"{Lead(winnerName)} ({winner.Priority}) at {winnerCulture} wins over {SourceLabel(next.Source)} ({next.Priority}) at {CultureLabel(next.Culture)}. A closer culture is used before a higher priority.";
    }

    private static string Lead(string name) =>
        name.Length == 0 ? name : char.ToUpperInvariant(name[0]) + name[1..];

    private static int Rank(IReadOnlyList<string> chain, string culture)
    {
        for (int index = 0; index < chain.Count; index++)
        {
            if (chain[index] == culture)
            {
                return index;
            }
        }

        return int.MaxValue;
    }

    private static string Heading(ILocalizationSource source) => source.Name switch
    {
        "LanguagePack" => $"Language pack ({source.Priority})",
        "Resx" => $"RESX ({source.Priority})",
        "Json" => $"JSON ({source.Priority})",
        "Database" => $"Database ({source.Priority})",
        _ => $"{source.Name} ({source.Priority})",
    };

    private static string SourceLabel(string name) => name switch
    {
        "LanguagePack" => "language pack",
        "Resx" => "RESX",
        "Json" => "JSON",
        "Database" => "database",
        _ => name,
    };

    private static string OnlySource(string name) => name switch
    {
        "language pack" => "Only the language pack has this key.",
        "database" => "Only the database has this key.",
        _ => $"Only {name} has this key.",
    };

    private static string CultureLabel(string culture) => culture.Length == 0 ? "invariant" : culture;

    private sealed record Contribution(string Source, int Priority, string Culture, string Text);

    private sealed record SourceCell(string? Text, string? CultureLabel, bool Wins);

    private sealed record DemoRow(string Key, string Reason, IReadOnlyList<SourceCell> Cells);
}
