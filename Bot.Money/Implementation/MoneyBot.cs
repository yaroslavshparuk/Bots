using Bot.Abstractions.Interfaces;
using Bot.Money.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace Bot.Money.Implementation
{
    public class MoneyBot : IBot
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IBudgetRepository _budgetRepository;
        public MoneyBot(string token, IBudgetRepository budgetRepository)
        {
            _botClient = new TelegramBotClient(token);
            _budgetRepository = budgetRepository;
        }

        public void Start()
        {
            _botClient.OnMessage += Bot_OnMessage;
            _botClient.StartReceiving();
        }

        public void Stop()
        {
            _botClient.StopReceiving();
        }

        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                var anyCommandWasExecuted = false;
                foreach (var command in GetAvailableCommands())
                {
                    if (command.CanExecute(e.Message))
                    {
                        await command.Execute(e.Message, _botClient);
                        anyCommandWasExecuted = true;
                        Console.WriteLine($"\n{DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")}: Proccessed message\n" +
                                          $"User Id: {e.Message.Chat.Id}\nUserName: @{e.Message.Chat.Username}");
                        break;
                    }
                }

                if (!anyCommandWasExecuted)
                {
                    throw new ArgumentException("Input is invalid");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n{DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")}: Can't process user's message: " +
                                  $"'{e.Message.Text}'\nUser Id: {e.Message.Chat.Id}\nUserName: @{e.Message.Chat.Username}");

                await _botClient.SendTextMessageAsync(e.Message.Chat, ex.Message, ParseMode.Default, false, false, 0);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"\n{DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")}: User does not exists! " +
                                  $"'{e.Message.Text}'\nUser Id: {e.Message.Chat.Id}\nUserName: @{e.Message.Chat.Username}");

                await _botClient.SendTextMessageAsync(e.Message.Chat, ex.Message, ParseMode.Default, false, false, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private IEnumerable<ICommand> GetAvailableCommands()
        {
            var commands = AppDomain.CurrentDomain.GetAssemblies()
                          .SelectMany(s => s.GetTypes())
                          .Where(p => typeof(ICommand).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract).ToArray();

            foreach (var command in commands)
            {
                if (command.GetConstructors().Any(t => t.GetParameters().Count() == 0))
                {
                    yield return (ICommand)Activator.CreateInstance(command);
                }
                else
                {
                    yield return (ICommand)Activator.CreateInstance(command, _budgetRepository);
                }
            }
        }
    }
}
