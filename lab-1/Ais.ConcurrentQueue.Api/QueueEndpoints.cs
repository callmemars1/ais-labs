using Ais.ConcurrentQueue.ConsumerPool;
using Ais.ConcurrentQueue.TaskQueue;
using Ais.ConcurrentQueue.Tasks;

namespace Ais.ConcurrentQueue.Api;

public static class QueueEndpoints
{
    public static IEndpointRouteBuilder AddQueueEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(
            pattern: "/tasks/count",
            handler: (ITaskQueue taskQueue) => taskQueue.Tasks.Count);

        builder.MapGet(
            pattern: "/tasks/list",
            handler: (ITaskQueue taskQueue) => taskQueue.Tasks.Select(selector: x => x.Id).ToArray());

        builder.MapPost(
            pattern: "/task/add",
            handler: (ITaskQueue taskQueue, int waitSeconds) =>
        {
            taskQueue.Produce(
                task: new WaitTask(id: Random.Shared.Next(), waitTime: TimeSpan.FromSeconds(value: waitSeconds)));

            return taskQueue.Tasks.Count;
        });

        builder.MapPost(
            pattern: "/task/remove",
            handler: (ITaskQueue taskQueue, int taskId) =>
        {
            taskQueue.Remove(taskId: taskId);
            return taskQueue.Tasks.Count;
        });

        builder.MapGet(
            pattern: "/consumers/count",
            handler: (IConsumerPool consumerPool) => consumerPool.ConsumersCount);

        builder.MapPost(
            pattern: "/consumer/add-one",
            handler: (IConsumerPool consumerPool) =>
        {
            consumerPool.AddConsumer();
            return consumerPool.ConsumersCount;
        });

        builder.MapPost(
            pattern: "/consumer/remove-one",
            handler: (IConsumerPool consumerPool) =>
        {
            consumerPool.RemoveConsumer();
            return consumerPool.ConsumersCount;
        });

        return builder;
    }
}