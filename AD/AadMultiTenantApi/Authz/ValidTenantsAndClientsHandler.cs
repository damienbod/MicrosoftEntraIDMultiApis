using Microsoft.AspNetCore.Authorization;

namespace AadMultiTenantApi;

public class ValidTenantsAndClientsHandler : AuthorizationHandler<ValidTenantsAndClientsRequirement>
{

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidTenantsAndClientsRequirement requirement)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (requirement == null)
            throw new ArgumentNullException(nameof(requirement));

        var scopeClaim = context.User.Claims.FirstOrDefault(t => t.Type == "scope");

        if (scopeClaim != null)
        {
            var scopes = scopeClaim.Value.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (scopes.Any(t => t == "access_as_user"))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}