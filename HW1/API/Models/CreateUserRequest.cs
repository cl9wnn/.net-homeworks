namespace API.Models;

/// <summary>
/// Модель запроса для создания нового пользователя
/// </summary>
/// <param name="Username">Имя пользователя (должно быть уникальным)</param>
/// <param name="Password">Пароль пользователя. Минимум 8 символов, должен содержать цифры и буквы в разных регистрах</param>
/// <param name="Email">Email пользователя. Должен быть валидным email-адресом</param>
public record CreateUserRequest(string Username, string Password, string Email);