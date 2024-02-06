using Ais.ConcurrentQueue.ConsumerPool;
using Ais.ConcurrentQueue.TaskQueue;
using Ais.ConcurrentQueue.Tasks;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ITaskQueue, ConcurrentTaskQueue>();
builder.Services.AddSingleton<IConsumerPool, ConsumerPool>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/tasks/count", (ITaskQueue taskQueue) => taskQueue.Tasks.Count);
app.MapGet("/tasks/list",  (ITaskQueue taskQueue) => taskQueue.Tasks.Select(x => x.Id).ToArray());
app.MapPost("/task/add",  (ITaskQueue taskQueue, int waitSeconds) =>
{
    taskQueue.Produce(new WaitTask(Random.Shared.Next(), TimeSpan.FromSeconds(waitSeconds)));
    return taskQueue.Tasks.Count;
});
app.MapPost("/task/remove",  (ITaskQueue taskQueue, int taskId) =>
{
    taskQueue.Remove(taskId);
    return taskQueue.Tasks.Count;
});
app.MapGet("/consumers/count", (IConsumerPool consumerPool) => consumerPool.ConsumersCount);
app.MapPost("/consumer/add-one", (IConsumerPool consumerPool) =>
{
    consumerPool.AddConsumer();
    return consumerPool.ConsumersCount;
});
app.MapPost("/consumer/remove-one", (IConsumerPool consumerPool) =>
{
    consumerPool.RemoveConsumer();
    return consumerPool.ConsumersCount;
});

app.Run();