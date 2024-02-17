using System.Collections.Immutable;
using Ais.ConcurrentQueue.ConsumerPool;
using Ais.ConcurrentQueue.TaskQueue;
using Ais.ConcurrentQueue.Tasks;

namespace Ais.ConcurrentQueue.Api;

public static class QueueEndpoints
{
    public record TaskStatus(int TaskId, string Status);

    public record TasksList(int TasksCount, TaskStatus[] Tasks);

    public record ProcessedTask(TaskStatus Status, TimeOnly StartProcessingTime, TimeOnly? EndProcessingTime);

    public record ProcessedTasksList(int ProcessedTasksCount, ProcessedTask[] ProcessedTasks);

    public record ConsumerStatus(int ConsumerId, ProcessedTasksList ProcessedTasks);

    public record ConsumerList(int ConsumersCount, ConsumerStatus[] Consumers);

    public static IEndpointRouteBuilder AddQueueEndpoints(this IEndpointRouteBuilder builder)
    {
        var queue = builder.MapGroup("/queue");
        queue.MapPost("/pause", PauseQueueProcessing);
        queue.MapPost("/resume", ResumeQueueProcessing);

        var queueTasks = queue.MapGroup("/tasks");
        queueTasks.MapGet(
            pattern: "/",
            handler: GetTasksInQueue);

        queueTasks.MapPost(
            pattern: "/add",
            handler: AddWaitTaskToQueue);

        queueTasks.MapPost(
            pattern: "/remove",
            handler: RemoveTaskFromQueue);

        var queueConsumers = queue.MapGroup("/consumers");
        queueConsumers.MapGet(
            pattern: "/",
            handler: GetConsumers);

        queueConsumers.MapPost(
            pattern: "/add",
            handler: AddConsumer);

        queueConsumers.MapPost(
            pattern: "/remove",
            handler: RemoveConsumer);

        return builder;
    }

    private static void ResumeQueueProcessing(ITaskQueue taskQueue)
    {
        taskQueue.ResumeProcessing();
    }

    private static void PauseQueueProcessing(ITaskQueue taskQueue)
    {
        taskQueue.PauseProcessing();
    }

    private static ConsumerList GetConsumers(IConsumerPool consumerPool)
    {
        var consumers = consumerPool
            .Consumers
            .Select(x =>
            {
                var tasksProcessedByConsumer = x.ProcessedTasks;
                return new ConsumerStatus(
                    x.Id,
                    new ProcessedTasksList(
                        ProcessedTasksCount: tasksProcessedByConsumer.Count(),
                        ProcessedTasks: tasksProcessedByConsumer
                            .Select(y => new ProcessedTask(
                                Status: new TaskStatus(y.Task.Id, y.Task.IsExecuted ? "Executed" : "Pending"),
                                StartProcessingTime: y.StartProcessingTime,
                                EndProcessingTime: y.EndProcessingTime))
                            .ToArray()));
            });

        return new ConsumerList(
            ConsumersCount: consumers.Count(),
            Consumers: consumers.ToArray());
    }

    private static int RemoveConsumer(IConsumerPool consumerPool)
    {
        consumerPool.RemoveConsumer();
        return consumerPool.Consumers.Count();
    }

    private static int AddConsumer(IConsumerPool consumerPool)
    {
        consumerPool.AddConsumer();
        return consumerPool.Consumers.Count();
    }

    private static TasksList GetTasksInQueue(ITaskQueue taskQueue) =>
        GetTasksList(taskQueue.Tasks);
    
    private static TasksList GetTasksList(IReadOnlyCollection<ITask> tasks) =>
        new(
            tasks.Count,
            tasks
                .Select(x => new TaskStatus(x.Id, x.IsExecuted ? "Executed" : "Pending"))
                .ToArray());

    private static TasksList AddWaitTaskToQueue(ITaskQueue taskQueue, int waitSeconds)
    {
        var newTaskId = Random.Shared.Next();
        var taskQueueSnapshot = taskQueue.Produce(new WaitTask(id: newTaskId, waitTime: TimeSpan.FromSeconds(value: waitSeconds)));

        return GetTasksList(taskQueueSnapshot);
    }

    private static TasksList RemoveTaskFromQueue(ITaskQueue taskQueue, int taskId)
    {
        var taskQueueSnapshot= taskQueue.Remove(taskId: taskId);
        return GetTasksList(taskQueueSnapshot);
    }
}