namespace API.Models;

/// <summary>
/// Ответ с информацией об ошибке
/// </summary>
/// <param name="ErrorMessage">Текстовое сообщение об ошибке</param>
public record ErrorResponse(string ErrorMessage);