namespace Bot.Abstractions.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> array, int size)
        {
            if (size <= 0) { throw new ArgumentException(); }

            for (var i = 0; i < (float)array.Count() / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }
}
