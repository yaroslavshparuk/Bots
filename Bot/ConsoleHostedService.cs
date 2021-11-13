using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System;
using Bot.Money.Repositories;
using System.Configuration;
using Bot.Money.Implementation;

namespace Bot
{
    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IBudgetRepository _budgetRepository;
        public ConsoleHostedService(ILogger<ConsoleHostedService> logger, IHostApplicationLifetime appLifetime, IBudgetRepository budgetRepository)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _budgetRepository = budgetRepository;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(() =>
                {
                    try
                    {
                        var moneyBot = new MoneyBot(ConfigurationManager.AppSettings["MoneyBotToken"], _budgetRepository);
                        moneyBot.Start();

                        Console.ReadKey();
                        moneyBot.Stop();
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
