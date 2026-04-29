namespace Howestprime.Movies.Application.Contracts.Data;

public class AuthorizationOptions
{
    public const string SectionName = "Authorization";
    public List<RolePermission> PermissionsByRole { get; set; } = [];
}

public class RolePermission
{
    public string Name { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = [];
}