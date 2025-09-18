using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public sealed class OwnerOrAdminRequirement : IAuthorizationRequirement { }

public sealed class OwnerOrAdminHandler : AuthorizationHandler<OwnerOrAdminRequirement>
{
    private readonly IHttpContextAccessor _http;
    private readonly ILogger<OwnerOrAdminHandler> _log;

    public OwnerOrAdminHandler(IHttpContextAccessor http, ILogger<OwnerOrAdminHandler> log)
    {
        _http = http;
        _log = log;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerOrAdminRequirement requirement)
    {
        // 1) Must be authenticated
        if (!(context.User.Identity?.IsAuthenticated ?? false))
            return Task.CompletedTask;

        // 2) Admin bypass
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 3) Get sub from token
        var sub = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                  ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(sub))
        {
            context.Fail(new AuthorizationFailureReason(this, "MissingSubClaim"));
            return Task.CompletedTask;
        }

        // 4) Get {memberId} from route
        var routeValues = _http.HttpContext?.Request.RouteValues;
        var routeId = routeValues != null && routeValues.TryGetValue("id", out var v)
            ? v?.ToString()
            : null;

        if (string.IsNullOrEmpty(routeId))
        {
            context.Fail(new AuthorizationFailureReason(this, "MissingRouteId"));
            return Task.CompletedTask;
        }

        // 5) Compare consistently (int↔int recommended)
        if (!int.TryParse(sub, out var subId) || !int.TryParse(routeId, out var memberId))
        {
            context.Fail(new AuthorizationFailureReason(this, "TypeMismatch"));
            return Task.CompletedTask;
        }

        if (subId != memberId)
        {
            context.Fail(new AuthorizationFailureReason(this, "DifferentUser"));
            return Task.CompletedTask;
        }

        // 6) Success
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
