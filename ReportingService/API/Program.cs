using API.Extensions;
using Application;
using Application.Abstractions;
using Core;
using Core.Abstractions;
using Core.Entities;
using Core.Events;
using Infrastructure.Database.Repositories;
using Infrastructure.Excel;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddPostgreSqlDb(builder.Configuration);
builder.Services.AddSingleton<IMessageHandler<UserRegisteredEvent>, UserRegisteredEventHandler>();
builder.Services.AddScoped<IUserRegistrationRepository, UserRegistrationRepository>();
builder.Services.AddKafkaConsumer<UserRegisteredEvent>(builder.Configuration.GetSection("Kafka:Users"));
builder.Services.AddExcelExporter<IExcelExportService<UserRegistration>, UserExcelExportService, UserRegistration>(
    builder.Configuration.GetSection("ExportSettings"));
builder.Services.AddQuartz(builder.Configuration.GetSection("Quartz:ExcelExportJob:CronSchedule"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.Services.ApplyMigrations();
}

app.Run();