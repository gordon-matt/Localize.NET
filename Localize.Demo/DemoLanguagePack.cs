namespace Localize.Demo;

public sealed class DemoLanguagePack : ILanguagePack
{
    public string? CultureCode => null;

    public IDictionary<string, string> LocalizedStrings { get; } = new Dictionary<string, string>
    {
        ["Demo/Home.Title"] = "Localization demo",
        ["Demo/Home.Body"] = "Hello from the language pack.",
        ["Demo/Home.Greeting"] = "Hello {0}",
    };
}
