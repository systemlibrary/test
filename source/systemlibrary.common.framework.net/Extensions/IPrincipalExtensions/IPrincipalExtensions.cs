using System.Security.Principal;

namespace SystemLibrary.Common.Framework.Extensions;

/// <summary>
/// Extension methods for <c>IPrincipal</c>.
/// </summary>
public static class IPrincipalExtensions
{
    /// <summary>
    /// Returns true if the principal is in any of the specified roles, case-sensitive.
    /// Enums are matched by key name — use <c>.ToValue()</c> on the enum to match by <c>EnumValue</c> instead.
    /// </summary>
    /// <example>
    /// <code>
    /// principal.IsInAnyRole("Admin", "Guest");
    /// principal.IsInAnyRole(AdminRoles.Admin, AdminRoles.MasterAdmin);
    /// </code>
    /// </example>
    public static bool IsInAnyRole(this IPrincipal principal, params object[] roles)
    {
        if (principal == null || roles == null) return false;

        for (int i = 0; i < roles.Length; i++)
            if (roles[i] != null && principal.IsInRole(roles[i] + ""))
                return true;

        return false;
    }
}
