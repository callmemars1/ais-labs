using Ais.ConcurrentQueue.Tasks;

namespace Ais.ConcurrentQueue.TaskQueue;

public interface ITaskQueue
{
    IReadOnlyCollection<ITask> Tasks { get; }
    
    IReadOnlyCollection<ITask> Produce(ITask task);

    ITask? Consume();

    IReadOnlyCollection<ITask> Remove(int taskId);
    
    public void PauseProcessing();
    
    public void ResumeProcessing();
}