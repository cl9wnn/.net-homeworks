using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenSearch;

namespace API.Extensions;

public static class ApplicationExtensions
{
    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
            $"{env.ApplicationName} v1"));
        
        return app;
    }
    
    public static IServiceProvider ApplyMigrations(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        try
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Ошибка при применении миграций.");
        }
        
        return services;
    }

    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        var openSearchUrl = builder.Configuration["OpenSearch:Url"] ?? string.Empty;
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationId()
            .Enrich.WithProperty("Application", "user-service")
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
            .WriteTo.Console()
            .WriteTo.OpenSearch(new OpenSearchSinkOptions(new Uri(openSearchUrl))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"user-service-logs-{DateTime.UtcNow:yyyy-MM}"
            })
            .CreateLogger();

        builder.Host.UseSerilog();
        
        return builder;
    }
}