using System.Configuration;
using System.Reflection;
using StackExchange.Redis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using log4net;
using log4net.Config;
using Coravel;
using Bot.Core.Abstractions;
using Bot.Youtube.Commands;
using Bot.Money.Repositories;
using Bot.Money.Commands;
using Bot.Money.Jobs;
using Bot.Money.Models;
using Bot.Money;
using Bot.Youtube;
using Telegram.Bot;

namespace Bot
{
    class StartUp
    {
        private readonly static ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect(
                                              ConfigurationManager.ConnectionStrings["redis"].ConnectionString);
        private static async Task Main(string[] args)
        {
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), new FileInfo("log4net.config"));
            var host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices((hostContext, services) =>
                 {
                     services.AddSingleton<IBot, MoneyBot>();
                     services.AddSingleton<IBot, YoutubeBot>();
                     services.AddTransient<IMoneyBotCommand, FinanceOperationCommand>();
                     services.AddTransient<IMoneyBotCommand, HelpCommand>();
                     services.AddTransient<IMoneyBotCommand, DownloadCommand>();
                     services.AddTransient<IYoutubeBotCommand, YoutubeVideoUrlToAudioCommand>();
                     services.AddTransient<IExportUrl, GoogleSpreadSheetsExportUrl>();
                     services.AddScoped<ICommandSteps, FinanceOperationCommandSteps>();
                     services.AddScoped<IUserDataRepository>(x => new RedisUserDataRepository(_redis));
                     services.AddScoped<IBudgetRepository, GoogleSpreadSheetsBudgetRepository>();
                     services.AddHostedService<ConsoleHostedService>();
                     services.AddScoped(x => new ResetMonthAndSendArchiveJob(
                                            x.GetService<IUserDataRepository>(),
                                            x.GetService<IBudgetRepository>(),
                                            new TelegramBotClient(ConfigurationManager.AppSettings["money_bot_token"])));
                     services.AddScheduler();
                 }).Build();

            host.Services.UseScheduler(scheduler =>
            {
                scheduler.Schedule<ResetMonthAndSendArchiveJob>().Cron("0 0 1 * *"); // At 00:00 on day-of-month 1
            });
            await host.RunAsync();
        }
    }
}
