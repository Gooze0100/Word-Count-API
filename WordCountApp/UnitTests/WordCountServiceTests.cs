using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WordCountAPI.Config;
using WordCountAPI.Services;

namespace UnitTests;

[TestFixture]
public class WordCountServiceTests
{
    private Mock<ILogger<WordCountService>> _loggerMock;
    private Mock<IOptions<AppSettings>> _optionsMock;
    private WordCountService _service;   
    
    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<WordCountService>>();
        _optionsMock = new Mock<IOptions<AppSettings>>();
        _optionsMock.Setup(o => o.Value).Returns(new AppSettings
        {
            AllowedFileExtensions = [".txt"],
            FileStorage = Path.GetTempPath()
        });

        _service = new WordCountService(_loggerMock.Object, _optionsMock.Object);
    }
    
    [Test]
    public async Task LoadFile_WithValidFile_ReturnsWordCount()
    {
        const string content = "hello word hello";
        const string fileName = "test.txt";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(stream.Length);
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

        var fileCollection = new FormFileCollection { fileMock.Object };

        var result = await _service.LoadFile(fileCollection, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Has.Count.EqualTo(1));
        Assert.That(result.Value[0].Id, Is.EqualTo(0));
        Assert.That(result.Value[0].FileName, Is.EqualTo(fileName));
        Assert.That(result.Value[0].WordOccurrences["hello"], Is.EqualTo(2));
        Assert.That(result.Value[0].WordOccurrences["word"], Is.EqualTo(1));
    }

    [Test]
    public async Task LoadFile_WithInvalidFileExtension_ReturnsFailure()
    {
        const string content = "hello word hello";
        const string fileName = "test.pdf";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(stream.Length);
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

        var fileCollection = new FormFileCollection { fileMock.Object };

        var result = await _service.LoadFile(fileCollection, CancellationToken.None);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.TypeOf<Exception>());
        Assert.That(result.Error.Message, Is.EqualTo("Invalid file extension"));
    }
    
     [Test]
        public async Task LoadFile_WithNoFiles_ReturnsFailure()
        {
            var files = new FormFileCollection();
            var result = await _service.LoadFile(files, CancellationToken.None);
            
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error.Message, Is.EqualTo("No files uploaded"));
        }

        [Test]
        public async Task LoadFile_WithTooManyFiles_ReturnsFailure()
        {
            var files = new FormFileCollection();
            for (int i = 0; i < 4; i++)
            {
                var stream = new MemoryStream("hello word hello"u8.ToArray());
                var fileMock = new Mock<IFormFile>();
                fileMock.Setup(f => f.FileName).Returns($"test{i}.txt");
                fileMock.Setup(f => f.Length).Returns(stream.Length);
                fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
                files.Add(fileMock.Object);
            }
            
            var result = await _service.LoadFile(files, CancellationToken.None);
            
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error.Message, Does.Contain("Too many files uploaded"));
        }

        [Test]
        public async Task LoadFile_WithEmptyFile_ReturnsError()
        {
            var stream = new MemoryStream();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.Length).Returns(0);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            var files = new FormFileCollection { fileMock.Object };
            
            var result = await _service.LoadFile(files, CancellationToken.None);
            
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error.Message, Does.Contain("File is empty"));
        }

        [Test]
        public async Task LoadFile_WithFileTooLarge_ReturnsError()
        {
            var largeSize = 1024 * 1024 * 6; // 6 MB
            var stream = new MemoryStream(new byte[largeSize]);
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.Length).Returns(largeSize);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            var files = new FormFileCollection { fileMock.Object };
            
            var result = await _service.LoadFile(files, CancellationToken.None);
            
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error.Message, Does.Contain("File size exceeds maximum"));
        }
}