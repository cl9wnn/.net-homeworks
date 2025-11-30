using API.Extensions;
using API.Middlewares;
using Core.Events;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddServices();
builder.Services.AddSwaggerDocumentation(builder.Environment);
builder.Services.AddValidation();
builder.Services.AddPostgreSqlDb(builder.Configuration);
builder.Services.AddMappings();
builder.Services.AddKafkaProducer<UserRegisteredEvent>(builder.Configuration.GetSection("Kafka:Users"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation(builder.Environment);
    app.Services.ApplyMigrations();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

app.Run();