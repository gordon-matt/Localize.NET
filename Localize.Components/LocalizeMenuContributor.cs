using Heroicsoft.NetForge.Components.Navigation;

namespace Localize.Components;

/// <summary>
/// Represents the localize menu contributor.
/// </summary>
public sealed class LocalizeMenuContributor : INetForgeMenuContributor
{
    /// <summary>
    /// The contributor name constant.
    /// </summary>
    public const string ContributorName = "Localize";

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name => ContributorName;

    /// <summary>
    /// Gets the default menu.
    /// </summary>
    public string DefaultMenu => "Settings";

    /// <summary>
    /// Performs the contribute operation.
    /// </summary>
    /// <param name="group">The group.</param>
    /// <param name="configure">The configure.</param>
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

/// <summary>
/// Provides extension methods for localize menus.
/// </summary>
public static class LocalizeMenuExtensions
{
    /// <summary>Adds the Languages link. The host can still retitle it.</summary>
    public static INetForgeMenuGroupBuilder AddLocalize(
        this INetForgeMenuGroupBuilder group,
        Action<NetForgeMenuItemOptions>? configure = null) =>
        group.AddContributor(LocalizeMenuContributor.ContributorName, configure);
}
