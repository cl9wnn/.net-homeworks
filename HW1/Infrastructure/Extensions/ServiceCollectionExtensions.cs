using Application.Abstractions;
using Infrastructure.Database;
using Infrastructure.Excel;
using Infrastructure.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgreSqlDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("PostgreSqlDbConnection"));
        });

        return services;
    }

    public static IServiceCollection AddQuartz(this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("ExcelExportJob");

            q.AddJob<ExcelExportJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("ExcelExportJob-trigger")
                .WithCronSchedule(configurationSection.Value!, x => x
                    .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"))));
        });

        services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });

        return services;
    }

    public static IServiceCollection AddExcelExporter<TService, TImplementation, TEntity>(
        this IServiceCollection services,
        IConfigurationSection configurationSection)
        where TService : class, IExcelExportService<TEntity>
        where TImplementation : class, TService
    { 
        services.Configure<ExportSettings>(configurationSection);
        services.AddScoped<TService, TImplementation>();

        return services;
    }
}