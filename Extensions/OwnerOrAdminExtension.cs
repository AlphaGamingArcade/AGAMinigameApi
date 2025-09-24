// Extension method for easy registration
using Microsoft.AspNetCore.Authorization;

public static class OwnerOnlyServiceExtensions
{
    public static IServiceCollection AddOwnerOrAdminAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, OwnerOrAdminHandler>();
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy("OwnerOrAdmin", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new OwnerOrAdminRequirement());
            });
        });

        return services;
    }
}
