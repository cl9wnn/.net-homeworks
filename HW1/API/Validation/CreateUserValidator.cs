using API.Models;
using FluentValidation;

namespace API.Validation;

public class CreateUserValidator: AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Несоответствующий почте формат");
        
        RuleFor(user => user.Username)
            .NotEmpty()
            .Matches("^[a-zA-Z0-9_\\s]{5,20}$")
            .WithMessage("Имя пользователя должно содержать от 5 до 20 символов");
        
        RuleFor(user => user.Password)
            .NotEmpty()
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
            .WithMessage("Пароль должен содержать от 8 до 20 символов, " +
                         "как минимум 1 цифра, специальный символ и большая буква");
    }
}