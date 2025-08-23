using System.Net;

namespace IntegrationTests.Tests.Cases;

[TestFixture]
public class WordCountTests
{
    [Test]
    public async Task WordCount_ReturnsSuccess()
    {
        var response = await IntegrationTestsSetup.HttpClient.PostAsync("/api/wordcount", null);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            
        using var formData = new MultipartFormDataContent();
        
        // formData.
        
    }
    
}