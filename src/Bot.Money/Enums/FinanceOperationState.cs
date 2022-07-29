namespace Bot.Money.Enums
{
    public enum FinanceOperationState
    {
        Started = 1,
        WaitingForType = 2,
        WaitingForCategory = 3,
        WaitingForDescription = 4,
        End = 5,
    }
}
