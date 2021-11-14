using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Configuration;
using Bot.Money.Repositories;
using Bot.Money.Implementation;
using Bot.Money.Commands;
using Bot.Money.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using log4net;
using System.Reflection;
using System.IO;
using log4net.Config;
using Bot.Core.Abstractions;

namespace Bot
{
    class StartUp
    {
        private static async Task Main(string[] args)
        {
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), new FileInfo("log4net.config"));
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
