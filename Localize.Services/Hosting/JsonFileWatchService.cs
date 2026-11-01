using Microsoft.Extensions.Hosting;

namespace Localize.Hosting;

internal sealed class JsonFileWatchService(
    IOptions<LocalizeOptions> options,
    JsonLocalizationSource source,
    ILocalizationCache cache,
    ILogger<JsonFileWatchService> logger) : IHostedService, IDisposable
{
    private FileSystemWatcher? watcher;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!options.Value.WatchJsonFiles)
        {
            return Task.CompletedTask;
        }

        string? directory = source.ResolveDirectory();
        if (directory is null || !Directory.Exists(directory))
        {
            return Task.CompletedTask;
        }

        watcher = new FileSystemWatcher(directory, "*.json")
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
            EnableRaisingEvents = true,
        };
        watcher.Changed += OnChanged;
        watcher.Created += OnChanged;
        watcher.Deleted += OnChanged;
        watcher.Renamed += OnChanged;
        watcher.Error += (_, args) => logger.LogDebug(args.GetException(), "JSON localization watcher stopped.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        watcher?.Dispose();
        watcher = null;
    }

    private void OnChanged(object sender, FileSystemEventArgs args)
    {
        source.Clear();
        cache.Invalidate();
    }
}
