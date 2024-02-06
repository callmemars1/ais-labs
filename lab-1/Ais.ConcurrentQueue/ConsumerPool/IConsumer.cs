namespace Ais.ConcurrentQueue.ConsumerPool;

public interface IConsumer
{
    public void StartConsuming();
    
    public void StopConsuming();
}