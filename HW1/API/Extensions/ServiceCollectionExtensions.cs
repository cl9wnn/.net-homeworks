using System.Reflection;
using API.Models;
using API.Validation;
using Application.Abstractions;
using Application.Services;
using Core.Abstractions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Database.Repositories;

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

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        
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