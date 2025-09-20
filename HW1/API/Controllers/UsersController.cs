using API.Entities;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Управление пользователями
/// </summary>
/// <param name="passwordHasher">Сервис для хеширования паролей</param>
/// <param name="users">Список пользователей в памяти</param>
[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class UsersController(IPasswordHasher<User> passwordHasher, List<User> users) : ControllerBase
{
    /// <summary>
    /// Получить список всех пользователей.
    /// </summary>
    /// <returns>Список пользователей в виде коллекции <see cref="UserResponse"/></returns>
    /// <response code="200"/>Возвращает список пользователей<response/>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var result = users.Select(u => new UserResponse(u.Id, u.Username, u.Email));

        return Ok(result);
    }

    /// <summary>
    /// Получить пользователей за указанный период времени.
    /// </summary>
    /// <param name="from">Начальная дата</param>
    /// <param name="to">Конечная дата</param>
    /// <returns>Список пользователей в виде коллекции <see cref="UserResponse"/></returns>
    /// <response code="200">Возвращает список пользователей</response>
    /// <response code="400">Начальная дата не может быть больше конечной</response>
    [HttpGet("filter")]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public IActionResult GetAllByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        if (from > to)
        {
            return BadRequest(new ErrorResponse("Начальная дата не может быть больше конечной"));
        }

        var result = users
            .Where(u => u.CreatedDate >= from && u.CreatedDate <= to ||
                        u.UpdatedDate >= from && u.UpdatedDate <= to)
            .Select(u => new UserResponse(u.Id, u.Username, u.Email));

        return Ok(result);
    }

    /// <summary>
    /// Получить пользователя по идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор пользователя</param>
    /// <returns>Объект <see cref="UserResponse"/></returns>
    /// <response code="200">Пользователь найден</response>
    /// <response code="404">Пользователь не найден</response>
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
    /// <returns>Созданный пользователь <see cref="UserResponse"/></returns>
    /// <response code="201">Пользователь успешно создан</response>
    /// <response code="400">Пользователь с таким username уже существует</response>
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
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        };

        user.Password = passwordHasher.HashPassword(user, request.Password);

        users.Add(user);

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, 
                new UserResponse(user.Id, user.Username, user.Email));
    }

    /// <summary>
    /// Обновить имя пользователя.
    /// </summary>
    /// <param name="id">Уникальный идентификатор пользователя</param>
    /// <param name="request">Модель запроса для обновления <see cref="UpdateUserRequest"/></param>
    /// <returns>Обновлённый объект <see cref="UserResponse"/></returns>
    /// <response code="200">Имя пользователя успешно обновлено</response>
    /// <response code="400">Имя пользователя уже занято</response>
    /// <response code="404">Пользователь не найден</response>
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
            return BadRequest(new ErrorResponse("Имя пользователя уже занято"));
        }

        user.Username = request.Username;
        user.UpdatedDate = DateTime.UtcNow;

        return Ok(new UserResponse(user.Id, user.Username, user.Email));
    }

    /// <summary>
    /// Удалить пользователя
    /// </summary>
    /// <param name="id">Уникальный идентификатор пользователя</param>
    /// <response code="204">Пользователь успешно удалён</response>
    /// <response code="404">Пользователь не найден</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
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