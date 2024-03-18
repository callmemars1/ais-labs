using System.Collections.Immutable;
using Ais.ConcurrentQueue.Tasks;

namespace Ais.ConcurrentQueue.TaskQueue;

public class ConcurrentTaskQueue : ITaskQueue
{
    private readonly LinkedList<ITask> _tasks = [];
    private volatile bool _isPaused; // atomic
    // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/variables#96-atomicity-of-variable-references

    public IReadOnlyCollection<ITask> Tasks
    {
        get
        {
            lock (_tasks)
            {
                return _tasks.ToImmutableList();
            }
        }
    }

    public IReadOnlyCollection<ITask> Produce(ITask task)
    {
        lock (_tasks)
        {
            _tasks.AddLast(task);
            Monitor.PulseAll(_tasks);
            return _tasks.ToImmutableList();
        }
    }

    public ITask Consume()
    {
        lock (_tasks)
        {
            while (_isPaused || _tasks.Count == 0) 
                Monitor.Wait(_tasks);

            var task = _tasks.First?.Value;
            _tasks.RemoveFirst();
            return task!;
        }
    }

    public void PauseProcessing()
    {
        lock (_tasks)
        {
            _isPaused = true;
        }
    }

    public void ResumeProcessing()
    {
        lock (_tasks)
        {
            _isPaused = false;
            Monitor.PulseAll(_tasks);
        }
    }

    public IReadOnlyCollection<ITask> Remove(int taskId)
    {
        lock (_tasks)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
                _tasks.Remove(task);

            return _tasks.ToImmutableList();
        }
    }
}