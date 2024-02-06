using Ais.ConcurrentQueue.TaskQueue;

namespace Ais.ConcurrentQueue.ConsumerPool;

public interface IConsumerPool
{
    int ConsumersCount { get; }
    
    void AddConsumer();

    void RemoveConsumer();
}

public class ConsumerPool(ITaskQueue taskQueue) : IConsumerPool
{
    private readonly List<IConsumer> _consumers = [];

    public int ConsumersCount => _consumers.Count;

    public void AddConsumer()
    {
        var consumer = new Consumer(taskQueue);
        _consumers.Add(consumer);
        consumer.StartConsuming();
    }

    public void RemoveConsumer()
    {
        if (_consumers.Count == 0)
            return;

        var consumer = _consumers.Last();
        consumer.StopConsuming();
        _consumers.Remove(consumer);
    }
}