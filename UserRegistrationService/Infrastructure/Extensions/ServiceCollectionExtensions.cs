using Application.Abstractions;
using Infrastructure.Database;
using Infrastructure.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
    
    public static void AddKafkaProducer<TMessage>(this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        services.Configure<KafkaOptions>(configurationSection);
        services.AddSingleton<IMessageProducer<TMessage>, KafkaProducer<TMessage>>();
    }
}