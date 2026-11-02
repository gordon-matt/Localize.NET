using Heroicsoft.NetForge.Abstractions.Auth;
using Localize;
using Heroicsoft.NetForge.Components;
using Heroicsoft.NetForge.Components.Layout;
using Heroicsoft.NetForge.Components.Navigation;
using Localize.Components;
using Localize.Components.Pages;
using Localize.Data.Sqlite;
using Localize.Demo;
using Localize.Demo.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();

builder.Services.AddLocalize(options =>
{
    options.RequireAuthentication = false;
    options.JsonDirectory = "Localization";
    options.TrackMissingKeys = true;
});
builder.Services.AddLocalizeSqlite();
builder.Services.AddLocalizeComponents();
builder.Services.AddSingleton<IAuthProviderService, DemoAuth>();
builder.Services.AddSingleton<ILanguagePack, DemoLanguagePack>();
builder.Services.AddHostedService<FrenchLanguageSeeder>();

builder.Services.Configure<ForgeShellOptions>(shell =>
{
    shell.Brand = "Localize";
    shell.LogoUrl = "img/Logo.png";
    shell.MarkUrl = "img/Icon.png";
    shell.UseSharedMenu = true;
    shell.ShowUserMenu = false;
});

builder.Services.BuildMenu(menu =>
{
    menu.AddItems().AddCustom(item =>
    {
        item.Title = "Home";
        item.Url = "/";
        item.Icon = "home";
        item.MatchAll = true;
    });
    menu.IncludeRegistered();
});

var app = builder.Build();

app.UseStaticFiles();
app.MapStaticAssets();
app.UseLocalize();
app.UseAntiforgery();
app.MapLocalize();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Languages).Assembly)
    .WithStaticAssets();

app.Run();
