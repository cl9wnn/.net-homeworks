using API.Entities;
using API.Extensions;
using API.Models;
using Microsoft.AspNetCore.Identity;

namespace API.Endpoints;

public static class UserEndpointsExtension
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder endpoints)
    {
        var users = new List<User>();

        var userGroup = endpoints.MapGroup("/users")
            .WithTags("Users");

        userGroup.MapGet("/", () =>
            {
                var result = users.Select(u => new UserResponse(u.Id, u.Username, u.Email));

                return Results.Ok(result);
            })
            .Produces<IEnumerable<UserResponse>>(StatusCodes.Status200OK);

        userGroup.MapGet("/{id:guid}", (Guid id) =>
            {
                var user = users.FirstOrDefault(u => u.Id == id);

                return user == null
                    ? Results.NotFound("Пользователь не найден!")
                    : Results.Ok(new UserResponse(user.Id, user.Username, user.Email));
            })
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status404NotFound);

        userGroup.MapPost("/login", (LoginUserRequest request, IPasswordHasher<User> hasher) =>
            {
                var user = users.FirstOrDefault(u => u.Username == request.Username);

                if (user == null)
                {
                    return Results.NotFound("Пользователь не найден!");
                }

                var verifyResult = hasher.VerifyHashedPassword(user, user.Password, request.Password);

                if (verifyResult == PasswordVerificationResult.Failed)
                {
                    return Results.Unauthorized();
                }

                return Results.Ok(new { token = "fake-jwt-token" });
            })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status401Unauthorized)
            .Produces<string>(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithValidation<LoginUserRequest>();

        userGroup.MapPost("/", (CreateUserRequest request, IPasswordHasher<User> hasher) =>
            {
                var userExists = users.Any(u => u.Username == request.Username);

                if (userExists)
                {
                    return Results.BadRequest("Пользователь с таким username уже существует");
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    Email = request.Email,
                };

                user.Password = hasher.HashPassword(user, request.Password);

                users.Add(user);

                return Results.Created(
                    $"/users/{user.Id}",
                    new UserResponse(user.Id, user.Username, user.Email)
                );
            })
            .Produces<UserResponse>(StatusCodes.Status201Created)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .WithValidation<CreateUserRequest>();

        userGroup.MapPatch("/{id:guid}", (Guid id, UpdateUserRequest request) =>
            {
                var user = users.FirstOrDefault(u => u.Id == id);

                if (user == null)
                {
                    return Results.NotFound("Пользователь не найден!");
                }

                var usernameExists = users.Any(u => u.Username == request.Username && u.Id != id);

                if (usernameExists)
                {
                    return Results.BadRequest("Username уже занят");
                }

                user.Username = request.Username;

                return Results.Ok(new UserResponse(user.Id, user.Username, user.Email));
            })
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithValidation<UpdateUserRequest>();

        userGroup.MapDelete("/{id:guid}", (Guid id) =>
            {
                var user = users.FirstOrDefault(u => u.Id == id);

                if (user == null)
                {
                    return Results.NotFound("Пользователь не найден!");
                }

                users.Remove(user);

                return Results.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<string>(StatusCodes.Status404NotFound);

        return endpoints;
    }
}