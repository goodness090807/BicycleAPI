using FluentValidation;

namespace BicycleAPI.Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode("Login.Email.NotEmpty").WithMessage("Email 不可為空")
            .EmailAddress().WithErrorCode("Login.Email.EmailAddressFormatError").WithMessage("Email 格式不正確");

        RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode("Login.Password.NotEmpty").WithMessage("密碼不可為空");
    }
}
