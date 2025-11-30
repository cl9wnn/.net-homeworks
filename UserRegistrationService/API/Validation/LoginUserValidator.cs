using API.Models;
using FluentValidation;

namespace API.Validation;

public class LoginUserValidator: AbstractValidator<LoginUserRequest>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Имя пользователя обязательно");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен");
    }
}