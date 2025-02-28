using Dometrain.Monolith.Api.Sdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;

var host = Host.CreateApplicationBuilder();


host.Services.AddDometrainApi("http://localhost:5148", "ThisIsAlsoMeantToBeSecret");

var app = host.Build();

var coursesApiClient = app.Services.GetRequiredService<ICourseApiClient>();

var course = await coursesApiClient.GetAsync("a4276985-3a5e-4de5-be3b-5a0a1f1262a9");

Console.WriteLine(course.Name);
