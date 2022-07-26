namespace Bot.Money.Enums
{
    internal enum FinanceOperationState
    {
        Started = 1,
        WaitingForType = 2,
        WaitingForCategory = 3,
        WaitingForDescription = 4,
        End,
    }
}
