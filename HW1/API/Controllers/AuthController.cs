using API.Entities;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IPasswordHasher<User> passwordHasher, List<User> users): ControllerBase
{
    /// <summary>
    /// Авторизовать пользователя по паролю
    /// </summary>
    /// <param name="request">Модель запроса для авторизации пользователя</param>
    /// <returns>JWT токен</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login(LoginUserRequest request)
    {
        var user = users.FirstOrDefault(u => u.Username == request.Username);

        if (user == null)
        {
            return NotFound(new ErrorResponse("Пользователь не найден!"));
        }

        var verifyResult = passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

        if (verifyResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized();
        }

        return Ok(new LoginResponse( "fake-jwt-token"));
    }
}