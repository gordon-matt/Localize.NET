using Heroicsoft.NetForge.Components.Navigation;

namespace Localize.Components;

public sealed class LocalizeMenuContributor : INetForgeMenuContributor
{
    public const string ContributorName = "Localize";

    public string Name => ContributorName;

    public string DefaultMenu => "Settings";

    public void Contribute(INetForgeMenuGroupBuilder group, Action<NetForgeMenuItemOptions>? configure)
    {
        group.AddCustom(item =>
        {
            item.Title = "Languages";
            item.Url = LocalizeRoutes.Languages;
            item.Icon = "globe";
            item.MatchAll = true;
            item.ActivePaths = [LocalizeRoutes.Languages, LocalizeRoutes.Strings];
            configure?.Invoke(item);
        });
    }
}

public static class LocalizeMenuExtensions
{
    /// <summary>Adds the Languages link. The host can still retitle it.</summary>
    public static INetForgeMenuGroupBuilder AddLocalize(
        this INetForgeMenuGroupBuilder group,
        Action<NetForgeMenuItemOptions>? configure = null) =>
        group.AddContributor(LocalizeMenuContributor.ContributorName, configure);
}
