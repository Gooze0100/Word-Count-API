using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using WordCountAPI.Models;

namespace IntegrationTests.Tests.Cases;

[Parallelizable(ParallelScope.All)]
public class WordCountTests
{
    [Test]
    public async Task WordCount_EmptyFileUpload_Returns400()
    {
        var httpClient = IntegrationTestsSetup.HttpClient;
        await IntegrationTestsSetup.SetAntiforgeryTokenAsync(httpClient);
        
        using var formData = new MultipartFormDataContent();
        
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "MockFiles", "EmptyFile.txt");
        await using var stream = File.OpenRead(filePath);
        IFormFile formFile = new FormFile(stream, 0, stream.Length, "file", "EmptyFile.txt");
        
        var fileContent = new StreamContent(formFile.OpenReadStream());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        formData.Add(fileContent, "file", formFile.FileName);
        
        var response = await httpClient.PostAsync("/api/wordcount", formData);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    
    [Test]
    public async Task WordCount_TextFileUpload_Returns200()
    {
        var httpClient = IntegrationTestsSetup.HttpClient;
        await IntegrationTestsSetup.SetAntiforgeryTokenAsync(httpClient);
        
        using var formData = new MultipartFormDataContent();
        
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "MockFiles", "FileWithText.txt");
        await using var stream = File.OpenRead(filePath);
        IFormFile formFile = new FormFile(stream, 0, stream.Length, "file", "FileWithText.txt");
        
        var fileContent = new StreamContent(formFile.OpenReadStream());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        formData.Add(fileContent, "file", formFile.FileName);
        
        var response = await httpClient.PostAsync("/api/wordcount", formData);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var result = await response.Content.ReadFromJsonAsync<List<EachWordOccurrences>>();
        Assert.That(result, Is.Not.Null);
    }
    
    [Test]
    public async Task WordCount_TextFileUpload_ReturnFailure()
    {
        var httpClient = IntegrationTestsSetup.HttpClient;
        await IntegrationTestsSetup.SetAntiforgeryTokenAsync(httpClient);
        
        using var formData = new MultipartFormDataContent();

        formData.Add(new ByteArrayContent([]), "file");
        
        var response = await httpClient.PostAsync("/api/", formData);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}