namespace Bot.Core.Abstractions
{
    public class ChatSession
    {
        private readonly Queue<(string, int)> _valueStates;

        public ChatSession(Queue<(string, int)> valueStates)
        {
            _valueStates = valueStates;
            CurrentState = _valueStates.Dequeue().Item2;
        }

        public int CurrentState { get; private set; }

        public int LastReplyId { get; private set; }

        public string LastTextMessage { get; private set; }

        public bool IsCompleted { get { return _valueStates.Count == 0; } }

        public void MoveNextState(string text, int id)
        {
            _valueStates.Enqueue((text, id));
            LastTextMessage = text;
            LastReplyId = id;
            CurrentState = _valueStates.Dequeue().Item2;
        }

        public IEnumerable<string> UnloadValues()
        {
            while(_valueStates.Count > 0)
            {
                yield return _valueStates.Dequeue().Item1;
            }
        }
    }
}
