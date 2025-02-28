using Microsoft.AspNetCore.Http;

namespace Dometrain.Api.Shared.Identity;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        var userId = context.User.Claims.SingleOrDefault(x => x.Type == "user_id");

        if (Guid.TryParse(userId?.Value, out var parsedId))
        {
            return parsedId;
        }

        return null;
    }
    
    public static bool IsAdmin(this HttpContext context)
    {
        var isAdmin = context.User.Claims.SingleOrDefault(x => x is { Type: "is_admin", Value: "true" });
        return isAdmin is not null;
    }
}
