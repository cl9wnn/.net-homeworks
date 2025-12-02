using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationExtensions
{
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
}