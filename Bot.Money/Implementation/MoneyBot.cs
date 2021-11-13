using Bot.Abstractions.Interfaces;
using Bot.Money.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace Bot.Money.Implementation
{
    public class MoneyBot : IBot
    {
        private readonly IEnumerable<IMoneyCommand> _commands;
        private ITelegramBotClient _botClient;
        public MoneyBot(IEnumerable<IMoneyCommand> commands)
        {
            _commands = commands;
        }

        public void Start()
        {
            var config = new ConfigurationBuilder()
             .AddJsonFile($"appsettings.json", true, true).Build();

            _botClient = new TelegramBotClient(config["bot_token"]);
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
                foreach (var command in _commands)
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

                await _botClient.SendTextMessageAsync(e.Message.Chat, ex.Message, ParseMode.Default, null, false, false, 0);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"\n{DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")}: User does not exists! " +
                                  $"'{e.Message.Text}'\nUser Id: {e.Message.Chat.Id}\nUserName: @{e.Message.Chat.Username}");

                await _botClient.SendTextMessageAsync(e.Message.Chat, ex.Message, ParseMode.Default, null, false, false, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
