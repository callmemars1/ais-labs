using System.Collections.Immutable;
using Ais.ConcurrentQueue.Tasks;

namespace Ais.ConcurrentQueue.TaskQueue;

public class ConcurrentTaskQueue : ITaskQueue
{
    private readonly LinkedList<ITask> _tasks = [];
    private readonly object _lock = new();

    public IReadOnlyCollection<ITask> Tasks
    {
        get
        {
            lock (_lock)
            {
                return _tasks.ToImmutableList();
            }
        }
    }

    public void Produce(ITask task)
    {
        lock (_lock)
        {
            _tasks.AddLast(task);
        }
    }

    public ITask? Consume()
    {
        lock (_lock)
        {
            var task = _tasks.Last?.Value;
            if (task == null)
                return null;

            _tasks.RemoveLast();
            return task;
        }
    }

    public void Remove(int taskId)
    {
        lock (_lock)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null) 
                _tasks.Remove(task);
        }
    }
}