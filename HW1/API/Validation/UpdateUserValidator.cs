using API.Models;
using FluentValidation;

namespace API.Validation;

public class UpdateUserValidator: AbstractValidator<UpdateUserRequest>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Имя пользователя обязательно")
            .MinimumLength(3).WithMessage("Имя пользователя должно содержать минимум 3 символа")
            .MaximumLength(50).WithMessage("Имя пользователя не должно превышать 50 символов")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Имя пользователя может содержать только буквы, цифры и подчеркивания");
    }
}