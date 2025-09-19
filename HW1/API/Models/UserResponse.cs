namespace API.Models;

/// <summary>
/// Ответ с информацией о пользователе
/// </summary>
/// <param name="Id">Уникальный идентификатор пользователя</param>
/// <param name="Username">Имя пользователя (должно быть уникальным)</param>
/// <param name="Email">Email пользователя. Должен быть валидным email-адресом</param>
public record UserResponse(Guid Id, string Username, string Email);