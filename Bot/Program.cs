using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Configuration;
using Bot.Money.Repositories;
using Bot.Money.Implementation;
using Bot.Money.Commands;
using Bot.Money.Interfaces;
using Bot.Abstractions.Interfaces;

namespace Bot
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IBot, MoneyBot>();
                    services.AddTransient<IMoneyCommand, FinanceOperationCommand>();
                    services.AddTransient<IMoneyCommand, HelpCommand>();
                    services.AddTransient<IMoneyCommand, ResetCommand>();
                    services.AddTransient<IMoneyCommand, ShowTypeCodesCommand>();
                    services.AddTransient<IUserDataRepository>(x => new RedisUserDataRepository(
                             ConnectionMultiplexer.Connect(ConfigurationManager.ConnectionStrings["redis"].ConnectionString).GetDatabase()));
                    services.AddTransient<IBudgetRepository, GoogleSpreadSheetsBudgetRepository>();
                    services.AddHostedService<ConsoleHostedService>();
                })
                .RunConsoleAsync();
        }
    }
}
