using API.Extensions;
using API.Middlewares;
using Application.Abstractions;
using Core.Entities;
using Infrastructure.Excel;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddServices();
builder.Services.AddSwaggerDocumentation(builder.Environment);
builder.Services.AddValidation();
builder.Services.AddPostgreSqlDb(builder.Configuration);
builder.Services.AddMappings();
builder.Services.AddExcelExporter<IExcelExportService<User>, UserExcelExportService, User>(
    builder.Configuration.GetSection("ExportSettings"));
builder.Services.AddQuartz(builder.Configuration.GetSection("Quartz:ExcelExportJob:CronSchedule"));
builder.ConfigureSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation(builder.Environment);
    app.Services.ApplyMigrations();
}

app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

app.Run();