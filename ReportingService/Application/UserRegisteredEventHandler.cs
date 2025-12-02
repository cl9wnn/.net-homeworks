using Core;
using Core.Abstractions;
using Core.Entities;
using Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application;

public class UserRegisteredEventHandler(ILogger<UserRegisteredEvent> logger, IServiceScopeFactory scopeFactory)
    : IMessageHandler<UserRegisteredEvent>
{
    public async Task HandleAsync(UserRegisteredEvent message, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRegistrationRepository>();
        
        var userRegistration = new UserRegistration
        {
            UserId = message.UserId,
            Username = message.Username,
            Email = message.Email,
            RegisteredAt = message.RegisteredAt,
        };

        await repository.Add(userRegistration);
        
        logger.LogInformation("User {username} is registered at {time}.", message.Username, message.RegisteredAt);
    }
}