using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Configuration;
using Bot.Money.Repositories;
using Bot.Money.Implementation;

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
                    services.AddTransient<IBudgetRepository, GoogleSpreadSheetsBudgetRepository>();
                    services.AddHostedService<ConsoleHostedService>();
                })
                .RunConsoleAsync();
        }
    }
}
