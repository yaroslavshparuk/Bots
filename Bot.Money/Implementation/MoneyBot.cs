using Bot.Abstractions.Interfaces;
using Bot.Money.Interfaces;
using Google;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace Bot.Money.Implementation
{
    public class MoneyBot : IBot
    {
        private readonly IEnumerable<IMoneyCommand> _commands;
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ITelegramBotClient _botClient;
        public MoneyBot(IEnumerable<IMoneyCommand> commands)
        {
            _commands = commands;
        }
        public void Start()
        {
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetCallingAssembly()), new FileInfo("log4net.config"));
            var config = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true).Build();

            _botClient = new TelegramBotClient(config["bot_token"]);
            _botClient.OnMessage += Bot_OnMessage;
            _botClient.StartReceiving();

            _logger.Info("Money bot was started");
        }

        public void Stop()
        {
            _botClient.StopReceiving();
            _logger.Info("Money bot was stoped");
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
                        _logger.Debug($"Proccessed message from: User Id: {e.Message.Chat.Id}UserName: @{e.Message.Chat.Username}");
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
                _logger.Error($"Can't process user's message: '{e.Message.Text}' User Id: {e.Message.Chat.Id} UserName: @{e.Message.Chat.Username}");
                await _botClient.SendTextMessageAsync(e.Message.Chat, ex.Message, ParseMode.Default, false, false, 0);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.Warn($"Unknown user: Message: '{e.Message.Text}' User Id: {e.Message.Chat.Id} UserName: @{e.Message.Chat.Username}");
                await _botClient.SendTextMessageAsync(e.Message.Chat, ex.Message, ParseMode.Default, false, false, 0);
            }
            catch(GoogleApiException ex)
            {
                _logger.Error(ex.Message);
                await _botClient.SendTextMessageAsync(e.Message.Chat, "Seems you provided wrong spread sheet URL", ParseMode.Default, false, false, 0);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }
    }
}
