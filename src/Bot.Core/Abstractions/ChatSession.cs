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

        public void MoveNext(string text)
        {
            _values.Enqueue(text);
            LastTextMessage = text;
            if(_states.TryDequeue(out var state))
            {
                CurrentState = state;
            }
            else
            {
                CurrentState = 1;
            }
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
