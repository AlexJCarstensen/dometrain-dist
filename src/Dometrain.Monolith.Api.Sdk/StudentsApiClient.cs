using System.Net;
using System.Net.Http.Json;
using Dometrain.Monolith.Api.Contracts.Responses;
using Refit;

namespace Dometrain.Monolith.Api.Sdk;

public interface IStudentsApiClient
{
    [Get("/students/{idOrEmail}")]
    Task<StudentResponse?> GetAsync(string idOrEmail);
}
