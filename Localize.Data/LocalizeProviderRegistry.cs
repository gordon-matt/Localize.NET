using Microsoft.Extensions.DependencyInjection;

namespace Localize.Data;

/// <summary>
/// Lets a database provider attach the storage services without referencing them.
/// <c>AddLocalize</c> sets <see cref="Attach"/>. Provider extensions invoke it, so either
/// registration order works.
/// </summary>
public static class LocalizeProviderRegistry
{
    /// <summary>
    /// Gets or sets the attach.
    /// </summary>
    public static Action<IServiceCollection>? Attach { get; set; }
}
