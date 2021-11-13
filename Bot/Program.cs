using Telegram.Bot;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Bot.Repositories.Interfaces;
using Bot.Repositories.Impl;
using Bot.Services.Interfaces;
using Bot.Services.Impl;
using StackExchange.Redis;
using Bot.Repository.Interfaces;
using Bot.Repository.Impl;
using System.Threading.Tasks;
using System.Configuration;

namespace Bot
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IUserDataRepository>(x => new RedisUserDataRepository(
                                                                    ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["RedisConnectionString"]).GetDatabase()
                                                              ));
                    services.AddSingleton<ITelegramBotClient>(x => new TelegramBotClient(ConfigurationManager.AppSettings["BudgetBotToken"]));
                    services.AddHostedService<ConsoleHostedService>();
                    services.AddTransient<IBot, BudgetBot>();
                    services.AddTransient<IBudgetRepository, GoogleSpreadSheetsBudgetRepository>();
                    services.AddTransient(x => new SheetsService(new BaseClientService.Initializer() { ApplicationName = "Monthly Budget" }));
                })
                .RunConsoleAsync();
        }
    }
}
