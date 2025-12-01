using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Kafka;

public static class ServiceCollectionExtensions
{
    public static void AddKafkaConsumer<TMessage>(this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        services.Configure<KafkaOptions>(configurationSection);
        services.AddHostedService<KafkaConsumer<TMessage>>();
    }
}