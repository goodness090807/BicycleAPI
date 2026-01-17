using FluentValidation;

namespace BicycleAPI.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode("Register.Email.NotEmpty").WithMessage("Email 不可為空")
            .EmailAddress().WithErrorCode("Register.Email.EmailAddressFormatError").WithMessage("Email 格式不正確");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithErrorCode("Register.DisplayName.NotEmpty").WithMessage("顯示名稱不可為空")
            .MaximumLength(100).WithErrorCode("Register.DisplayName.MaximumLength").WithMessage("顯示名稱長度不可超過 100 個字元");
        RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode("Register.Password.NotEmpty").WithMessage("密碼不可為空")
            .MinimumLength(6).WithErrorCode("Register.Password.MinimumLength").WithMessage("密碼長度至少需要 6 個字元");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithErrorCode("Register.ConfirmPassword.Equal").WithMessage("確認密碼與密碼不符");
    }
}
