namespace Bot.Money.Enums
{
    internal enum FinanceOperationState
    {
        NotStarted = -1,
        Started = 1,
        WaitingForType = 2,
        WaitingForCategory = 3,
        WaitingForDescription = 4,
    }
}
