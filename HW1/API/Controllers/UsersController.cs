using API.Models;
using Application.Abstractions;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Управление пользователями
/// </summary>
/// <param name="userService">Сервис для работы с пользователями</param>
[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    /// <summary>
    /// Получить список всех пользователей.
    /// </summary>
    /// <returns>Список пользователей в виде коллекции <see cref="UserResponse"/></returns>
    /// <response code="200"/>Возвращает список пользователей<response/>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync()
    {
        var users = await userService.GetAllAsync();

        return Ok(users.Select(u => new UserResponse(u.Id, u.Username, u.Email)));
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
    public async Task<IActionResult> GetAllByDateRangeAsync([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        if (from > to)
        {
            return BadRequest(new ErrorResponse("Начальная дата не может быть больше конечной"));
        }

        var users = await userService.GetAllByDateRangeAsync(from, to);

        return Ok(users.Select(u => new UserResponse(u.Id, u.Username, u.Email)));
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
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var user = await userService.GetAsync(id);

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
    public async Task<IActionResult> RegisterAsync(CreateUserRequest request)
    {
        var userDto = new UserDto
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };

        var createdUser = await userService.AddAsync(userDto);

        if (createdUser == null)
        {
            return BadRequest("Пользователь с таким username уже существует");
        }

        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdUser.Id },
            new UserResponse(createdUser.Id, createdUser.Username, createdUser.Email));
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
    public async Task<IActionResult> UpdateUsernameAsync(Guid id, UpdateUserRequest request)
    {
        var user = await userService.GetAsync(id);

        if (user == null)
        {
            return NotFound(new ErrorResponse("Пользователь не найден!"));
        }

        var updatedUserDto = new UserDto
        {
            Id = user.Id,
            Username = request.Username,
        };

        var updatedUser = await userService.UpdateAsync(updatedUserDto);

        if (updatedUser == null)
        {
            return BadRequest(new ErrorResponse("Имя пользователя уже занято"));
        }

        return Ok(new UserResponse(updatedUser.Id, updatedUser.Username, updatedUser.Email));
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
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var isDeleted = await userService.DeleteAsync(id);

        return isDeleted ? NoContent() : NotFound("Пользователь не найден");
    }
}