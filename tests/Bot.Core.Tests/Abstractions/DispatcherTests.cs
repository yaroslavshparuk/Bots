﻿using Bot.Core.Abstractions;
using Bot.Money.Handlers;
using Bot.Money.Services;
using Bot.Money.Enums;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Xunit;
using Bot.Core.Exceptions;
using Bot.Money.Repositories;
using Bot.Money.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Message = Bot.Core.Abstractions.Message;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using Telegram.Bot.Requests;

namespace Bot.Core.Tests.Abstractions
{
    public class DispatcherTests
    {
        private readonly IChatSessionService _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;
        private readonly Mock<IBudgetRepository> _budgetRepository;
        private readonly IMemoryCache _memoryCache;
        private IList<IMoneyBotInputHandler> _handlers;

        public DispatcherTests()
        {
            _chatSessionService = new ChatSessionService();
            _botClient = new Mock<ITelegramBotClient>();
            _budgetRepository = new Mock<IBudgetRepository>();
            _handlers = new List<IMoneyBotInputHandler>();
            var services = new ServiceCollection();
            services.AddMemoryCache();
            _memoryCache = services.BuildServiceProvider().GetService<IMemoryCache>();
        }

        [Fact]
        public async void DispatchInputIsAmountReturnWaitingForTypeMessage()
        {
            _botClient.Setup(x => x.MakeRequestAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new Telegram.Bot.Types.Message()));
            _handlers = new List<IMoneyBotInputHandler> { new FinOpsAmountEntered() };
            var dispatcher = new Dispatcher(_handlers, _chatSessionService, _botClient.Object);
            var message = new Message(123, "test", "123");

            await dispatcher.Dispatch(message);
            var session = _chatSessionService.GetOrCreate(message.ChatId);

            Assert.Equal((int)FinanceOperationState.WaitingForType, session.CurrentState);
            Assert.Equal(message.Text, session.LastTextMessage);
        }

        [Fact]
        public async void DispatchInputIsNotAmountThrowNotFoundCommandException()
        {
            var dispatcher = new Dispatcher(_handlers, _chatSessionService, _botClient.Object);
            var message = new Message(123, "test", "Not amount");
            await Assert.ThrowsAsync<NotFoundCommandException>(() => dispatcher.Dispatch(message));
        }

        [Fact]
        public async Task FullFinanceOperationTest()
        {
            _botClient.Setup(x => x.MakeRequestAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new Telegram.Bot.Types.Message()));
            _budgetRepository.Setup(x => x.GetCategories(123, "Expense")).Returns(Task.FromResult(new string[] { "Food" }.AsEnumerable()));
            _handlers = new List<IMoneyBotInputHandler>()
            {
                new FinOpsAmountEntered(),
                new FinOpsTypeEntered(_budgetRepository.Object, _memoryCache),
                new FinOpsCategoryEntered(_budgetRepository.Object, _memoryCache),
                new FinOpsDescriptionEntered(_budgetRepository.Object),
            };

            var dispatcher = new Dispatcher(_handlers, _chatSessionService, _botClient.Object);
            var testMessages = new Message[] {
                new Message(123, "test", "123"),
                new Message(123, "test", "Expense"),
                new Message(123, "test", "Food"),
                new Message(123, "test", "Banana"),
            };

            foreach (var m in testMessages)
            {
                await dispatcher.Dispatch(m);
            }

            _budgetRepository.Verify(x => x.CreateRecord(It.IsAny<FinanceOperationMessage>()), Times.Once());
        }
    }
}
