using Dometrain.Monolith.Api.Contracts.Responses;
using Refit;

namespace Dometrain.Monolith.Api.Sdk;

public interface ICourseApiClient
{
    [Get("/courses/{idOrSlug}")]
    Task<CourseResponse?> GetAsync(string idOrSlug);
}





