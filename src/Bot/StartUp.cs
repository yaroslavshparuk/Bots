using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Configuration;
using Bot.Money.Repositories;
using Bot.Money.Impl;
using Bot.Money.Commands;
using Bot.Money.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using log4net;
using System.Reflection;
using log4net.Config;
using Bot.Core.Abstractions;
using Bot.Youtube.Impl;
using Bot.Youtube.Interfaces;
using Bot.Youtube.Commands;
using Coravel;
using Bot.Money.Jobs;

namespace Bot
{
    class StartUp
    {
        private static ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect(
                                              ConfigurationManager.ConnectionStrings["redis"].ConnectionString);

        private static async Task Main(string[] args)
        {
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), new FileInfo("log4net.config"));
            var host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices((hostContext, services) =>
                 {
                     services.AddSingleton<IBot, MoneyBot>();
                     services.AddSingleton<IBot, YoutubeBot>();

                     services.AddTransient<IMoneyCommand, FinanceOperationCommand>();
                     services.AddTransient<IMoneyCommand, HelpCommand>();
                     services.AddTransient<IMoneyCommand, ShowTypeCodesCommand>();
                     services.AddTransient<IMoneyCommand, DownloadCommand>();
                     services.AddTransient<IYoutubeCommand, YoutubeVideoUrlToAudioCommand>();

                     services.AddScoped<IUserDataRepository>(x => new RedisUserDataRepository(_redis));
                     services.AddScoped<IBudgetRepository, GoogleSpreadSheetsBudgetRepository>();
                     services.AddHostedService<ConsoleHostedService>();

                     services.AddScheduler();
                     services.AddTransient<ResetMonthAndSendArchiveJob>();
                 }).Build();

            host.Services.UseScheduler(scheduler =>
            {
                scheduler.Schedule<ResetMonthAndSendArchiveJob>().Cron("5 0 1 * *"); // At 00:05 on day-of-month 1
            });
            await host.RunAsync();
        }
    }
}
