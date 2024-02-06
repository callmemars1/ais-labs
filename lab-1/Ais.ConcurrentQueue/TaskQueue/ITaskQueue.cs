using Ais.ConcurrentQueue.Tasks;

namespace Ais.ConcurrentQueue.TaskQueue;

public interface ITaskQueue
{
    IReadOnlyCollection<ITask> Tasks { get; }
    
    void Produce(ITask task);

    ITask? Consume();

    void Remove(int taskId);
}