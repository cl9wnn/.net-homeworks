namespace API.Models;

/// <summary>
/// Ответ при успешной аутентификации
/// </summary>
/// <param name="Token">JWT токен</param>
public record LoginResponse(string Token);