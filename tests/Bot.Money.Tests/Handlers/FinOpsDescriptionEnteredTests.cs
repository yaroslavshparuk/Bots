using Bot.Core.Abstractions;
using Bot.Core.Exceptions;
using Bot.Money.Handlers;
using Bot.Money.Models;
using Bot.Money.Repositories;
using Bot.Money.Services;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace Bot.Money.Tests.Handlers
{
    public class FinOpsDescriptionEnteredTests
    {
        private readonly IChatSessionService _chatSessionService;
        private readonly Mock<ITelegramBotClient> _botClient;
        private readonly Mock<IBudgetRepository> _budgetRepository;

        public FinOpsDescriptionEnteredTests()
        {
            _chatSessionService = new ChatSessionService();
            _botClient = new Mock<ITelegramBotClient>();
            _budgetRepository = new Mock<IBudgetRepository>();
        }

        [Fact]
        public void IsSuitableInputIsStartedStateSessionReturnsFalse()
        {
            var handler = new FinOpsDescriptionEntered(_budgetRepository.Object);
            var textMessage = new Message { Text = "Expense", Chat = new Chat { Id = 123 } };
            var session = _chatSessionService.DownloadOrCreate(textMessage.Chat.Id);
            Assert.False(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public void IsSuitableInputIsWaitingForDescriptionStateSessionReturnsTrue()
        {
            var handler = new FinOpsDescriptionEntered(_budgetRepository.Object);
            var textMessage = new Message { Text = "Apples", Chat = new Chat { Id = 123 } };
            var session = _chatSessionService.DownloadOrCreate(textMessage.Chat.Id);
            session.MoveNext("123");
            session.MoveNext("Expense");
            session.MoveNext("Food");
            Assert.True(handler.IsSuitable(new UserRequest(session, textMessage, _botClient.Object)));
        }

        [Fact]
        public async Task HandleInputIsStringThenVerifySendTextMessageAsyncAndCreateRecordWereCalled()
        {
            var handler = new FinOpsDescriptionEntered(_budgetRepository.Object);
            var textMessage = new Message { Text = "Apples", Chat = new Chat { Id = 123 } };
            var session = _chatSessionService.DownloadOrCreate(textMessage.Chat.Id);
            session.MoveNext("123");
            session.MoveNext("Expense");
            session.MoveNext("Food");
            await handler.Handle(new UserRequest(session, textMessage, _botClient.Object));
            _budgetRepository.Verify(x => x.CreateRecord(It.IsAny<FinanceOperationMessage>()), Times.Once());
        }
    }
}
