namespace Bot.Core.Abstractions
{
    public class ChatSession
    {
        private readonly Queue<int> _states;
        private readonly Queue<string> _values;

        public ChatSession(Queue<int> states)
        {
            _states = states;
            CurrentState = _states.Dequeue();
            _values = new Queue<string>();
        }

        public int CurrentState { get; private set; }

        public string LastTextMessage { get; private set; }

        public bool IsCompleted { get { return _states.Count == 0; } }

        public void MoveNext(string text)
        {
            _values.Enqueue(text);
            LastTextMessage = text;
            CurrentState = _states.Dequeue();
        }

        public IEnumerable<string> UnloadValues()
        {
            while(_values.Count > 0)
            {
                yield return _values.Dequeue();
            }
        }
    }
}
