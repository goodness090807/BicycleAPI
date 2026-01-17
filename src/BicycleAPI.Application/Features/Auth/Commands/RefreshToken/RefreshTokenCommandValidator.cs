using FluentValidation;

namespace BicycleAPI.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithErrorCode("RefreshToken.NotEmpty").WithMessage("Refresh Token 不能為空。");
    }
}
