using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Extensions;

public static class ValidationFilterExtensions
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            var arg = context.Arguments.OfType<T>().FirstOrDefault();

            if (arg == null)
            {
                return Results.BadRequest("Invalid request payload");
            }

            var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

            if (validator == null)
            {
                return Results.Problem($"No validator registered for {typeof(T).Name}");
            }

            var result = await validator.ValidateAsync(arg);

            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }

            return await next(context);
        });
        
        return builder;
    }
}