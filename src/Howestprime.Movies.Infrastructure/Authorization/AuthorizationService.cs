using Microsoft.Extensions.Options;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Contracts.Data;

namespace Howestprime.Movies.Infrastructure.Authorization;

public class AuthorizationService(IOptions<AuthorizationOptions> options) : IAuthorizationService
{
    private readonly AuthorizationOptions _options = options.Value;

    public void Authorize(string role, string permission)
    {
        var roleConfig = _options.PermissionsByRole
            .FirstOrDefault(r => r.Name.Equals(role, StringComparison.OrdinalIgnoreCase));

        if (roleConfig == null || !roleConfig.Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException($"Role '{role}' is not authorized for '{permission}'.");
        }
    }
}