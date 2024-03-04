class PriorityQueue<T>
{
    private SortedDictionary<int, Queue<T>> dict = new SortedDictionary<int, Queue<T>>();

    public void Enqueue(T item, int priority)
    {
        if (!dict.ContainsKey(priority))
            dict[priority] = new Queue<T>();
        dict[priority].Enqueue(item);
    }

    public T Dequeue()
    {
        var pair = dict.First();
        var item = pair.Value.Dequeue();
        if (pair.Value.Count == 0)
            dict.Remove(pair.Key);
        return item;
    }

    public int Count => dict.Count;
}
