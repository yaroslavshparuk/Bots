using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Bot.Services.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Bot
{
    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IBot _bot;
        public ConsoleHostedService(ILogger<ConsoleHostedService> logger, IHostApplicationLifetime appLifetime, IBot bot)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _bot = bot;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Starting with arguments: {string.Join("10 ", Environment.GetCommandLineArgs())}");
            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(() =>
                {
                    try
                    {
                        _logger.LogInformation("Starting bot...");

                        _bot.Start();

                        Console.WriteLine("Press any key to stop");
                        Console.ReadKey();

                        _bot.Stop();
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
