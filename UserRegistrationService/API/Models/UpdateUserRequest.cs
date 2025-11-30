namespace API.Models;

/// <summary>
/// Модель запроса для обновления имени пользователя
/// </summary>
/// <param name="Username">Имя пользователя (должно быть уникальным)</param>
public record UpdateUserRequest(string Username);
