using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Evolve.Genius
{
    internal sealed record ConsoleHostedService(IHostApplicationLifetime AppLifetime, ILogger<ConsoleHostedService> Logger) : IHostedService
    {
        private int? _exitCode;

        public Task StartAsync(CancellationToken ct)
        {
            AppLifetime.ApplicationStarted.Register(() =>
            {
                try
                {
                    Logger.LogInformation("Hello world");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Unhandled exception!");
                    _exitCode = 1;
                }
                finally
                {
                    AppLifetime.StopApplication();
                }
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken ct)
        {
            Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
            return Task.CompletedTask;
        }
    }
}
