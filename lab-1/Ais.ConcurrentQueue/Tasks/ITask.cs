namespace Ais.ConcurrentQueue.Tasks;

public interface ITask
{
    int Id { get; }
    
    Task Execute(CancellationToken cancellationToken);
    
    bool IsExecuted { get; }
}