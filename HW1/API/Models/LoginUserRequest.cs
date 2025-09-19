namespace API.Models;

/// <summary>
/// Модель запроса для аутентификации пользователя
/// </summary>
/// <param name="Username">Имя пользователя (должно быть уникальным)</param>
/// <param name="Password">Пароль пользователя. Минимум 8 символов, должен содержать цифры и буквы в разных регистрах</param>
public record LoginUserRequest(string Username, string Password);
