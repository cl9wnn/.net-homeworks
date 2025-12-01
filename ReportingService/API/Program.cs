using Application;
using Core;
using Core.Events;
using Infrastructure.Kafka;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IMessageHandler<UserRegisteredEvent>, UserRegisteredEventHandler>();
builder.Services.AddKafkaConsumer<UserRegisteredEvent>(builder.Configuration.GetSection("Kafka:Users"));

var app = builder.Build();

app.Run();