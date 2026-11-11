using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using SheriffPersonnel.Shared.Data;
 
namespace SheriffPersonnel.Web.Services;
 
public class UserClaimsTransformer : IClaimsTransformation
{
    private readonly DBContext _db;
    private readonly ILogger<UserClaimsTransformer> _logger;
 
    public UserClaimsTransformer(DBContext db, ILogger<UserClaimsTransformer> logger)
    {
        _db = db;
        _logger = logger;
    }
 
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
 
        if (identity == null || !identity.IsAuthenticated)
        {
            return principal;
        }
 
        try
        {
            var email = identity.FindFirst(ClaimTypes.Email)?.Value
                     ?? identity.FindFirst("preferred_username")?.Value;
 
            var oid = identity.FindFirst("oid")?.Value
                   ?? identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
 
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(oid))
            {
                return principal;
            }
 
            var query = _db.Users.AsNoTracking();
            if (!string.IsNullOrEmpty(oid) && !string.IsNullOrEmpty(email))
            {
                query = query.Where(u => u.EntraObjectID == oid || u.UserPrincipalName == email);
            }
            else if (!string.IsNullOrEmpty(oid))
            {
                query = query.Where(u => u.EntraObjectID == oid);
            }
            else
            {
                query = query.Where(u => u.UserPrincipalName == email);
            }
 
            var user = await query
                .Select(u => new
                {
                    u.UserID,
                    u.DisplayName,
                    u.UserPrincipalName,
                    Roles = u.UserRoles
                        .Where(ur => ur.Role != null)
                        .Select(ur => ur.Role!.RoleName)
                        .ToList()
                })
                .FirstOrDefaultAsync();
 
            if (user != null)
            {
                if (!identity.HasClaim(c => c.Type == "UserID"))
                {
                    identity.AddClaim(new Claim("UserID", user.UserID.ToString()));
                }
                if (!identity.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                {
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()));
                }

                foreach (var role in user.Roles)
                {
                    if (!string.IsNullOrEmpty(role))
                    {
                        if (!identity.HasClaim(ClaimTypes.Role, role))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String));
                        }
                        if (!string.IsNullOrEmpty(identity.RoleClaimType) && identity.RoleClaimType != ClaimTypes.Role && !identity.HasClaim(identity.RoleClaimType, role))
                        {
                            identity.AddClaim(new Claim(identity.RoleClaimType, role, ClaimValueTypes.String));
                        }
                        if (!identity.HasClaim("roles", role))
                        {
                            identity.AddClaim(new Claim("roles", role, ClaimValueTypes.String));
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while transforming user claims from database.");
        }
 
        return principal;
    }
}
 