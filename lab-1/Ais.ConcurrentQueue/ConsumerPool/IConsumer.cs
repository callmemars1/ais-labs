using Ais.ConcurrentQueue.Tasks;

namespace Ais.ConcurrentQueue.ConsumerPool;

public interface IConsumer
{
    public int Id { get; }
    
    public IEnumerable<ProcessedTask> ProcessedTasks { get; }
    
    public void StartConsuming();
    
    public void StopConsuming();
}

public record ProcessedTask(TimeOnly StartProcessingTime, TimeOnly? EndProcessingTime, ITask Task)
{
    public TimeOnly? EndProcessingTime { get; set; } = EndProcessingTime;
}