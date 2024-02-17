using System.Collections.Concurrent;
using Ais.ConcurrentQueue.TaskQueue;

namespace Ais.ConcurrentQueue.ConsumerPool;

public class Consumer(int id, ITaskQueue taskQueue) : IConsumer
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ConcurrentStack<ProcessedTask> _processedTasks = new();

    public int Id { get; } = id;

    public IEnumerable<ProcessedTask> ProcessedTasks => _processedTasks.ToArray();

    public void StartConsuming()
    {
        Task.Run(() =>
        {
            while (true)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                    return;
                
                var task = taskQueue.Consume();
                if (task == null)
                    continue;

                var processedTask = new ProcessedTask(
                    StartProcessingTime: TimeOnly.FromDateTime(DateTime.Now),
                    EndProcessingTime: null,
                    Task: task);
                _processedTasks.Push(processedTask);
                Console.WriteLine($"[{processedTask.StartProcessingTime:h:mm:ss tt}]: Task {task.Id} is STARTED on CONSUMER {Id} AND THREAD {Environment.CurrentManagedThreadId}");
                task.Execute(_cancellationTokenSource.Token).Wait();
                processedTask.EndProcessingTime = TimeOnly.FromDateTime(DateTime.Now);
                Console.WriteLine($"[{processedTask.EndProcessingTime:h:mm:ss tt}]: Task {task.Id} is ENDED on CONSUMER {Id} AND THREAD {Environment.CurrentManagedThreadId}");
            }
        }, cancellationToken: _cancellationTokenSource.Token);
    }

    public void StopConsuming()
    {
        _cancellationTokenSource.Cancel();
    }
}