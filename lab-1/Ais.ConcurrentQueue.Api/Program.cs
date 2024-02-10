using Ais.ConcurrentQueue.Api;
using Ais.ConcurrentQueue.ConsumerPool;
using Ais.ConcurrentQueue.TaskQueue;

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

app.AddQueueEndpoints();

app.Run();