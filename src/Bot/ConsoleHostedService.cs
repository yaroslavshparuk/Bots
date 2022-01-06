using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Bot.Core.Abstractions;

namespace Bot
{
    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IEnumerable<IBot> _bots;
        public ConsoleHostedService(ILogger<ConsoleHostedService> logger, IHostApplicationLifetime appLifetime, IEnumerable<IBot> bots)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _bots = bots;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(() =>
                {
                    try
                    {
                        foreach (var bot in _bots)
                        {
                            bot.Start();
                        }

                        Console.ReadKey();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled exception!");
                    }
                    finally
                    {
                        _appLifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
