using System.Net;
using Dometrain.Monolith.Api.Sdk;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;

namespace Microsoft.Extensions.DependencyInjection;

public static class Extensions
{
    public static IServiceCollection AddDometrainApi(this IServiceCollection services,
        string baseUrl, string apiKey)
    {
        services.AddHttpClient("dometrain-api", client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        });

        services.AddRefitClient<ICourseApiClient>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(baseUrl);
                c.DefaultRequestHeaders.Add("x-api-key", apiKey);
            }).AddResilienceHandler("custompolicy", builder =>
            {
                builder.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 2,
                    ShouldHandle = static args => ValueTask.FromResult(args is
                    {
                        Outcome.Result.StatusCode: HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests
                    }),
                    Delay = TimeSpan.FromSeconds(2),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true
                });
            });
        
        services.AddRefitClient<IStudentsApiClient>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(baseUrl);
                c.DefaultRequestHeaders.Add("x-api-key", apiKey);
            }).AddStandardResilienceHandler();
        return services;
    }
}
