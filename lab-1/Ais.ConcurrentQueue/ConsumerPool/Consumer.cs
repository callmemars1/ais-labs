using Ais.ConcurrentQueue.TaskQueue;

namespace Ais.ConcurrentQueue.ConsumerPool;

public class Consumer(ITaskQueue taskQueue) : IConsumer
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public void StartConsuming()
    {
        Task.Run(() =>
        {
            Console.WriteLine("Consumer started");
            while (true)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                    return;
                
                var task = taskQueue.Consume();
                task?.Execute(_cancellationTokenSource.Token);
            }
        }, cancellationToken: _cancellationTokenSource.Token);
    }

    public void StopConsuming()
    {
        _cancellationTokenSource.Cancel();
        Console.WriteLine("Consumer stopped");
    }
}