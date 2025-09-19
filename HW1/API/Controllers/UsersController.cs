using API.Entities;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IPasswordHasher<User> passwordHasher, List<User> users): ControllerBase
{
    /// <summary>
    /// Получить список всех пользователей.
    /// </summary>
    /// <returns>Список пользователей в виде коллекции <see cref="UserResponse"/></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var result = users.Select(u => new UserResponse(u.Id, u.Username, u.Email));

        return Ok(result);
    }

    /// <summary>
    /// Получить пользователя по идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор пользователя</param>
    /// <returns>
    /// Объект <see cref="UserResponse"/> если пользователь найден,
    /// либо сообщение об ошибке, если не найден.
    /// </returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var user = users.FirstOrDefault(u => u.Id == id);

        return user == null
            ? NotFound(new ErrorResponse("Пользователь не найден!"))
            : Ok(new UserResponse(user.Id, user.Username, user.Email));
    }

    /// <summary>
    /// Зарегистрировать пользователя.
    /// </summary>
    /// <param name="request">Модель запроса для создания пользователя <see cref="CreateUserRequest"/></param>
    /// <returns>
    /// Созданный пользователь <see cref="UserResponse"/> с кодом 201;
    /// либо ошибка 400, если имя пользователя уже существует.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public IActionResult Register(CreateUserRequest request)
    {
        var userExists = users.Any(u => u.Username == request.Username);

        if (userExists)
        {
            return BadRequest(new ErrorResponse("Пользователь с таким username уже существует"));
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
        };

        user.Password = passwordHasher.HashPassword(user, request.Password);

        users.Add(user);

        return Created(
            $"api/users/{user.Id}",
            new UserResponse(user.Id, user.Username, user.Email)
        );
    }

    /// <summary>
    /// Обновить имя пользователя.
    /// </summary>
    /// <param name="id">Уникальный идентификатор пользователя</param>
    /// <param name="request">Модель запроса для обновления <see cref="UpdateUserRequest"/></param>
    /// <returns>
    /// Обновлённый объект <see cref="UserResponse"/> если успешно;
    /// Ошибка 404, если пользователь не найден;
    /// Ошибка 400, если имя пользователя уже занято.
    /// </returns>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public IActionResult UpdateUsername(Guid id, UpdateUserRequest request)
    {
        var user = users.FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            return NotFound(new ErrorResponse("Пользователь не найден!"));
        }

        var usernameExists = users.Any(u => u.Username == request.Username && u.Id != id);

        if (usernameExists)
        {
            return BadRequest(new ErrorResponse("Username уже занят"));
        }

        user.Username = request.Username;

        return Ok(new UserResponse(user.Id, user.Username, user.Email));
    }
    
    /// <summary>
    /// Удалить пользователя
    /// </summary>
    /// <param name="id">Уникальный идентификатор пользователя</param>
    /// <returns>
    /// Код 204, если пользователь успешно удалён;
    /// Ошибка 404, если пользователь не найден.
    /// </returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType( StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse),  StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var user = users.FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            return NotFound(new ErrorResponse("Пользователь не найден!"));
        }

        users.Remove(user);

        return NoContent();
    }
}