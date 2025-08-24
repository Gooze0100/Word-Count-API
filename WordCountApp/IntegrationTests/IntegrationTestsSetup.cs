using System.Net.Http.Headers;
using AutoFixture;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WordCountAPI.Config;

namespace IntegrationTests;

[SetUpFixture]
public static class IntegrationTestsSetup
{
    private static WebApplicationFactory<Program> Factory { get; set; }
    public static HttpClient HttpClient => CreateTestClient();
    private static Fixture Fixture { get; set; }
    
    static IntegrationTestsSetup()
    {
        Fixture = new Fixture();
        Fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => Fixture.Behaviors.Remove(b));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
    }
    
    [OneTimeSetUp]
    public static void Setup()
    {
        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Testing";

                builder.UseEnvironment(environment)
                    .UseTestServer();
            });
    }
    
    private static HttpClient CreateTestClient()
    {
        if (Factory == null)
        {
            throw new InvalidOperationException("Factory is null");
        }
        
        var httpClient = Factory.CreateClient();
        var appSettings = Factory.Services.GetRequiredService<IOptions<AppSettings>>().Value;

        if (!string.IsNullOrWhiteSpace(appSettings.Token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", appSettings.Token);
        }
        
        return httpClient;
    }
    
    public static async Task SetAntiforgeryTokenAsync(HttpClient httpClient)
    {
        if (httpClient == null)
        {
            throw new InvalidOperationException("HttpClient is null");
        }
        
        var token = await httpClient.GetAsync("/api/antiforgery/token");
        token.EnsureSuccessStatusCode();
    }
    
    [OneTimeTearDown]
    public static void TearDown()
    {
        HttpClient?.Dispose();
        Factory?.Dispose();
    }
}