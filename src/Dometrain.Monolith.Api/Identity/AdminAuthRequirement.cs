using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Dometrain.Monolith.Api.Identity;

public class AdminAuthRequirement : IAuthorizationHandler, IAuthorizationRequirement
{
    public const string ApiUserId = "005d25b1-bfc8-4391-b349-6cec00d1416c";
    private readonly string _apiKey;

    public AdminAuthRequirement(string apiKey)
    {
        _apiKey = apiKey;
    }

    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.User.HasClaim("is_admin", "true"))
        {
            context.Succeed(this);
            return Task.CompletedTask;
        }
        
        var httpContext = context.Resource as HttpContext;
        if (httpContext is null)
        {
            return Task.CompletedTask;
        }
        
        if (!httpContext.Request.Headers.TryGetValue("x-api-key", out
                var extractedApiKey))
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        if (_apiKey != extractedApiKey)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var identity = (ClaimsIdentity)httpContext.User.Identity!;
        identity.AddClaim(new Claim("user_id", Guid.Parse(ApiUserId).ToString()));
        identity.AddClaim(new Claim("is_admin", "true"));
        context.Succeed(this);
        return Task.CompletedTask;
    }
}
