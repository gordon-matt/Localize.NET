# Localize

Database, JSON, and RESX translations behind one `IStringLocalizer`, with a Blazor admin built on NetForge.

A string is resolved from the request culture, then each parent culture, then the invariant text. Within one culture the sources are merged by priority. A higher number wins:

| Source | Priority | Writes |
|---|---|---|
| Language packs (`ILanguagePack`) | 10 | No |
| RESX | 40 | No |
| JSON files | 50 | No |
| Database | 100 | Yes, from the admin |

Set `Localize:JsonOverridesDatabase` when a deployed file should beat a stored row.

The invariant culture is stored as an empty culture code. A missing translation falls through. Deleting a translation removes the row, so the fallback works again.

## Add it to a site

```csharp
builder.Services.AddLocalize(options =>
{
    options.JsonDirectory = "Localization";
    options.TrackMissingKeys = true; // Inferno's discover-on-first-use
});
builder.Services.AddLocalizeSqlite(); // or AddLocalizeSqlServer / AddLocalizeNpgsql / AddLocalizeMySql
builder.Services.AddLocalizeComponents();

// existing language pack classes only change their namespace
builder.Services.AddSingleton<ILanguagePack, MyLanguagePack>();
```

```csharp
app.UseLocalize();   // replaces UseRequestLocalization
app.MapLocalize();   // culture cookie and JSON export

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Localize.Components.Pages.Languages).Assembly);
```

Point `ForgeRouter.AdditionalAssemblies` at the same assembly. Drop the link in with `AddLocalize()` on a menu group, or `IncludeRegistered()`, which files it under Settings.

Admin routes:

- `/localize/languages`
- `/localize/strings/{cultureCode}`

`AddLocalize` applies the database migrations on startup (`AutoMigrate`) and, for the global scope, inserts `DefaultCulture` (en-US) as an enabled language.

Call `AddLocalize` and the provider method in either order. Both are required for the admin. JSON and RESX still resolve with no database, but the editor and `ILanguageService` need one.

## Multi-tenant scope

`Language`, `LocalizableString`, and `LocalizableProperty` are partitioned by a string scope. A single-tenant site leaves it empty.

```csharp
builder.Services.AddLocalize<TenantLocalizationScope>(options => options.TrackMissingKeys = true);

public sealed class TenantLocalizationScope(IWorkContext work) : ILocalizationScope
{
    public string? GetScopeId() => work.CurrentTenant?.Id.ToString();
}
```

Call `ILanguageService.EnsureDefaultAsync()` when a tenant is created. The startup seed only covers the global scope.

## Language packs

Same shape Inferno already uses. `CultureCode` null is the invariant pack.

```csharp
public sealed class MyLanguagePack : ILanguagePack
{
    public string? CultureCode => null;

    public IDictionary<string, string> LocalizedStrings { get; } = new Dictionary<string, string>
    {
        ["App/Home.Title"] = "Home",
    };
}
```

`T["App/Home.Title"]` reads that text until an editor, a JSON file, or a RESX overrides it. `T["App/Home.Greeting", name]` formats with the current UI culture. The string editor lists every key those sources know, and saving writes a database override for the current scope. Reset on the languages page deletes the scope's stored strings and inserts the packs again. Files and RESX are left alone.

## JSON

`Localize:JsonDirectory` is relative to the content root.

- `Localization/fr-FR.json` or `Localization/fr-FR/Home.json` is French. Nested objects flatten with dots (`Home.Title`).
- `default.json` or `invariant.json` is the invariant culture.
- A file named `Home.json` in the root of that folder is ignored, so a stray JSON file is not treated as a culture.
- The admin import also accepts Inferno's export: `cultureCode` plus `localizedStrings`. A culture the file introduces is added switched off.

## RESX

```csharp
builder.Services.AddLocalizeResx<SharedResources>();
```

The type needs the generated public static `ResourceManager`. RESX is read-only. Parent cultures are not applied twice; the catalog walks them.

## Entity fields

UI strings and translated entity fields are different tables, as they were in Mantle and Inferno. The entity keeps the invariant value. A row in `LocalizableProperties` replaces one property for one culture.

```csharp
string? title = await properties.ValueOrDefaultAsync(
    cultureCode, typeof(Page).FullName!, page.Id.ToString(), "Title", page.Title);
```

`FindAsync` loads a batch. `DeleteForEntityAsync` removes every culture when the entity is deleted. Scope is included, so two tenants with the same entity id do not collide.

## Replacing Inferno

1. Remove the Inferno localization projects from the host and reference `Localize.Components` plus one `Localize.Data.*` provider.
2. Register `ILocalizationScope` from the current tenant, as above, and set `TrackMissingKeys`.
3. Change language pack namespaces from `Inferno.Localization` to `Localize`. The interface is the same shape.
4. Replace `AddInfernoLocalization()` and `UseRequestLocalization()` with `AddLocalize`, `UseLocalize`, and `MapLocalize`. The cookie name stays `CurrentCulture`, so visitors keep their choice.
5. Add the components assembly to the router and point the Localization menu item at `/localize/languages`.
6. Content blocks call `ILocalizablePropertyService` instead of the Inferno service. `ValueOrDefaultAsync` covers the single-field read. The batch method takes the same culture, entity type, ids, and property name.
7. Plan a data move from `inferno.Languages`, `inferno.LocalizableStrings`, and `inferno.LocalizableProperties`. This library uses the `localize` schema, int keys, a string scope (the tenant id as text), and an empty string for the invariant culture. It does not read the old tables in place.

`LocalizedDisplayNameAttribute` and `LocalizedHelpTextAttribute` still take a catalog key. They resolve `IStringLocalizer` once the host has started.

## Options

Bound from the `Localize` section.

| Option | Default | |
|---|---|---|
| `DefaultCulture` | `en-US` | Used when the visitor has not chosen, and seeded as a language |
| `CultureCookieName` | `CurrentCulture` | |
| `UseParentCultureFallback` | true | `fr-FR` then `fr` then invariant |
| `TrackMissingKeys` | false | Store a key the first time a page asks for it |
| `JsonDirectory` | empty | No JSON until this is set |
| `JsonOverridesDatabase` | false | |
| `AutoMigrate` | true | |
| `SeedDefaultLanguage` | true | Global scope only |
| `RequireAuthentication` | true | Admin pages. The culture switch stays public |
| `ManagePolicy` | empty | Set this to an existing policy such as Inferno's languages policy |
| `ImportOnlyKnownKeys` | false | When true, import will not create keys that have no invariant row |

## Layout

`Localize.Core` has no database and no UI. Packs and scopes reference that. `Localize.Services` is the catalog and the localizer. `Localize.Data` plus `Localize.Data.Sql`, `.Npgsql`, `.MySql`, and `.Sqlite` follow the same split as EasyChatz: mappings in one project, migrations in the provider project, and repositories bound to `ILocalizeDbContextFactory` so they do not take over the host's `IDbContextFactory`. `Localize.Components` is the Razor class library. `Localize.Demo` is a small host that shows the switcher and the admin.
