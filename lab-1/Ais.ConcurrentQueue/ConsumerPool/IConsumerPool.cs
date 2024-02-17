using System.Collections.Immutable;
using Ais.ConcurrentQueue.TaskQueue;

namespace Ais.ConcurrentQueue.ConsumerPool;

public interface IConsumerPool
{
    IEnumerable<IConsumer> Consumers { get; }
    
    void AddConsumer();

    void RemoveConsumer();
}

public class ConsumerPool(ITaskQueue taskQueue) : IConsumerPool
{
    private readonly List<IConsumer> _consumers = [];

    public IEnumerable<IConsumer> Consumers => _consumers.ToImmutableArray();

    public void AddConsumer()
    {
        var consumer = new Consumer(
            id: Random.Shared.Next(),
            taskQueue: taskQueue);

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