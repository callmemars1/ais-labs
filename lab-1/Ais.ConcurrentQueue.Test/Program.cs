using Ais.ConcurrentQueue.ConsumerPool;
using Ais.ConcurrentQueue.TaskQueue;
using Ais.ConcurrentQueue.Tasks;

var taskQueue = new ConcurrentTaskQueue();
var consumerPool = new ConsumerPool(taskQueue);

// every consumer runs in it's own thread
consumerPool.AddConsumer();
consumerPool.AddConsumer();
consumerPool.AddConsumer();
consumerPool.AddConsumer();

var firstProduceThread = new Thread(() => ProduceTasks(taskQueue, 0, 120));
var secondProduceThread = new Thread(() => ProduceTasks(taskQueue, 120, 120));

firstProduceThread.Start();
secondProduceThread.Start();

firstProduceThread.Join();
secondProduceThread.Join();

Console.WriteLine("===================== Waiting 65 seconds. All consumers should complete");
Thread.Sleep(TimeSpan.FromSeconds(65));
Console.WriteLine("===================== Waiting 65 seconds done.");

var allProcessedTasks = consumerPool
    .Consumers
    .SelectMany(t => t.ProcessedTasks);

Console.WriteLine($"Processed tasks count: {allProcessedTasks.Count()}"); // should be 240

foreach (var consumer in consumerPool.Consumers)
{
    Console.WriteLine($"Consumer [{consumer.Id}] processed:");
    foreach (var task in consumer.ProcessedTasks)
        Console.WriteLine($"[{task.Task.Id}] Task: ${task.StartProcessingTime:HH:mm:ss} - ${task.EndProcessingTime:HH:mm:ss}. Executed: {task.Task.IsExecuted}");
}

return;


void ProduceTasks(ITaskQueue queue,int start, int count)
{
    foreach (var index in Enumerable.Range(start, count))
    {
        var waitTime = TimeSpan.FromSeconds(1);
        queue.Produce(new WaitTask(++start, waitTime));
    }
}