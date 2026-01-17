using BicycleAPI.Domain.Repositories;
using MediatR;
using Shared.ResultPatterns;

namespace BicycleAPI.Application.Features.Auth.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
        {
            return Result<GetUserByIdResponse>.Failure(
                new NotFoundError("User.NotFound", "User", "使用者未找到。")
            );
        }

        var userDto = new GetUserByIdResponse(user.Id, user.Email, user.DisplayName);

        return Result<GetUserByIdResponse>.Success(userDto);
    }
}
