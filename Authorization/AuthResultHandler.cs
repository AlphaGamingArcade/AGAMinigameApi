using AGAMinigameApi.Dtos.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

public sealed class AuthResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _fallback = new();

    public async Task HandleAsync(RequestDelegate next, HttpContext ctx, AuthorizationPolicy policy, PolicyAuthorizationResult result)
    {
        if (result.Challenged)
        {
            await _fallback.HandleAsync(next, ctx, policy, result);
            return;
        }

        if (result.Forbidden)
        {
            var code = result.AuthorizationFailure?.FailureReasons.FirstOrDefault()?.Message ?? "Forbidden";
            var message = code switch
            {
                "MissingSubClaim"      => "Token has no 'sub' claim.",
                "MissingRouteId" => "Route is missing 'id'.",
                "TypeMismatch"         => "Token sub and route memberId have different types.",
                "DifferentUser"        => "You are not authorized to access this resource.",
                _                      => "Insufficient permissions."
            };

            var response = new ApiResponse<object>(
                true,
                "Forbidden",
                message,
                StatusCodes.Status403Forbidden
            );

            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            await ctx.Response.WriteAsJsonAsync(response);
            return;
        }

        await _fallback.HandleAsync(next, ctx, policy, result);
    }
}
