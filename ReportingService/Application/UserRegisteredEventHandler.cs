using Core;
using Core.Events;
using Microsoft.Extensions.Logging;

namespace Application;

public class UserRegisteredEventHandler(ILogger<UserRegisteredEvent> logger) : IMessageHandler<UserRegisteredEvent>
{
    public Task HandleAsync(UserRegisteredEvent message, CancellationToken cancellationToken)
    {
        logger.LogInformation("User {username} is registered at {time}.", message.Username, message.RegisteredAt);
        
        return Task.CompletedTask;
    }
}