using BicycleAPI.Api.Extensions;
using BicycleAPI.Application.Features.Permissions.Queries.CheckUserPermission;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace BicycleAPI.Api.Authorizations.PermissionAuthorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IMediator _mediator;
    public PermissionAuthorizationHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var userId = context.User.GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return;
        }

        var result = await _mediator.Send(new CheckUserPermissionQuery(userId, requirement.PermissionName));
        if (result.IsSuccess && result.Value)
        {
            context.Succeed(requirement);
        }

        await Task.CompletedTask;
    }
}
