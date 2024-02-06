namespace Ais.ConcurrentQueue.Tasks;

public class WaitTask(int id, TimeSpan waitTime) : ITask
{
    public int Id { get; } = id;

    public Task Execute(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Task {Id} is STARTED on thread {Environment.CurrentManagedThreadId}...");
        IsExecuted = false;
        var task = Task.Delay(waitTime, cancellationToken);
        task.Wait(cancellationToken);
        IsExecuted = true;
        Console.WriteLine($"Task {Id} is FINISHED on thread {Environment.CurrentManagedThreadId}...");
        return task;
    }

    public bool IsExecuted { get; private set; }
}