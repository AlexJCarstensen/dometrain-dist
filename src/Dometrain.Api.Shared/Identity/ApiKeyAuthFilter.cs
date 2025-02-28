using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Dometrain.Api.Shared.Identity;

public class ApiKeyAuthFilter : IAuthorizationFilter
{
    private readonly IOptions<IdentitySettings> _identitySettings;

    public ApiKeyAuthFilter(IOptions<IdentitySettings> identitySettings)
    {
        _identitySettings = identitySettings;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("x-api-key",
                out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult("API Key missing");
            return;
        }
        
        if (_identitySettings.Value.AdminApiKey != extractedApiKey)
        {
            context.Result = new UnauthorizedObjectResult("Invalid API Key");
        }
    }
}
