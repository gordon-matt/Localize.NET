using Microsoft.Extensions.Hosting;

namespace Localize.Hosting;

internal sealed class LocalizeRuntimeInitializer(IServiceProvider services) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        LocalizeRuntime.Services = services;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
