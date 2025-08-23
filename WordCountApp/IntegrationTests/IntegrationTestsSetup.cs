using AutoFixture;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace IntegrationTests;

[SetUpFixture]
public static class IntegrationTestsSetup
{
    public static WebApplicationFactory<Program> Factory { get; private set; }
    public static HttpClient HttpClient => CreateClient();
    public static Fixture Fixture { get; private set; }
    
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
    
    private static HttpClient CreateClient()
    {
        var httpClient = Factory.CreateClient();
        // httpClient.DefaultRequestHeaders.Add(Constants.HeaderKeys.EditingUserName, TestContext.CurrentContext.Test.Name);

        return httpClient;
    }
    
    [OneTimeTearDown]
    public static void TearDown()
    {
        HttpClient?.Dispose();
        Factory?.Dispose();
    }
}