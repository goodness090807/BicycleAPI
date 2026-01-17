using BicycleAPI.Api.Authorizations.PermissionAuthorization;
using BicycleAPI.Api.Extensions;
using BicycleAPI.Application.Features.Auth.Queries.GetUserById;
using BicycleAPI.Domain.Variables.Permissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BicycleAPI.Api.Controllers;

public class UsersController : ApiController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 查詢目前登入使用者資料
    /// </summary>
    /// <returns></returns>
    [HttpGet, Authorize]
    [PermissionRequirement(Permissions.User.View)]
    public async Task<IActionResult> GetUserByAuthorizationId()
    {
        var result = await _mediator.Send(new GetUserByIdQuery(User.GetUserIdFromClaims()));
        return HandleResult(result);
    }
}
