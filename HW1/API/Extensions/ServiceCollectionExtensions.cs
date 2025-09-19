using System.Reflection;
using API.Models;
using API.Validation;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IWebHostEnvironment env)
    {

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = env.ApplicationName, Version = "v1" });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        
        return services;
    }

    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateUserRequest>, CreateUserValidator>();
        services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserValidator>();
        services.AddScoped<IValidator<LoginUserRequest>, LoginUserValidator>();

        services.AddFluentValidationAutoValidation();

        return services;
    }
}