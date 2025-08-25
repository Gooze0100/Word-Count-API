using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using WordCountAPI.Config;
using WordCountAPI.Models;

namespace WordCountAPI.Services;

public class WordCountService : IWordCountService
{
    private readonly long _maxFileSize = 1024 * 1024 * 5; // 5MB
    private readonly int _maxAllowedFiles = 3;
    
    private readonly ILogger<WordCountService> _logger;
    private readonly IOptions<AppSettings> _settings;
    
    public WordCountService(ILogger<WordCountService> logger,  IOptions<AppSettings> settings)
    {
        _logger = logger;
        _settings = settings;
    }
    
    public async Task<Result<List<EachWordOccurrences>, Exception>> LoadFile(IFormFileCollection files, CancellationToken cancellationToken)
    {
        try
        {
            if (files.Count == 0)
            {
                _logger.LogError("No files uploaded");
                return new Exception("No files uploaded");
            }
            
            if (files.Count > _maxAllowedFiles)
            {
                _logger.LogError("Too many files uploaded. Maximum allowed is {MaxAllowedFiles}", _maxAllowedFiles);
                return new Exception($"Too many files uploaded. Maximum allowed is {_maxAllowedFiles}");
            }
            
            string filesFolder = _settings.Value.FileStorage;

            if (!Directory.Exists(filesFolder))
            {
                Directory.CreateDirectory(filesFolder);
            }

            List<EachWordOccurrences> result = new();
                
            foreach (var (file, index) in files.Select((f, i) => (f, i)))
            {
                string[] allowedFileExtensions = _settings.Value.AllowedFileExtensions;
                var fileExtension = Path.GetExtension(file.FileName);
                if (!allowedFileExtensions.Contains(fileExtension))
                {
                    _logger.LogError("Uploaded {FileName} with extension: {Extension} which is incorrect",  file.FileName, fileExtension);
                    return new Exception("Invalid file extension");
                }
            
                if (file.Length == 0)
                {
                    _logger.LogError("Uploaded {FileName} is empty",  file.FileName);
                    return new Exception("File is empty");
                }

                if (file.Length > _maxFileSize)
                {
                    _logger.LogError("Uploaded {FileName} with length: {Length} is too big",  file.FileName, file.Length);
                    return new Exception($"File size exceeds maximum allowed size of {_maxFileSize}, you file size {file.Length}");
                }
                
                string newFileName = Path.ChangeExtension(
                    Path.GetRandomFileName(), 
                    Path.GetExtension(file.FileName));
                
                string path = Path.Combine(
                    filesFolder,
                    newFileName
                );

                await using (FileStream fs = new(path, FileMode.Create))
                {
                    await file.OpenReadStream().CopyToAsync(fs, cancellationToken);
                };
                
                string text = await File.ReadAllTextAsync(path, cancellationToken);
                result.Add(new EachWordOccurrences()
                {
                    Id = index,
                    FileName = file.FileName,
                    WordOccurrences = WordCount(text)
                });
            }

            _logger.LogInformation("File successfully created");
            return Result.Success<List<EachWordOccurrences>, Exception>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return ex;
        }
    }

    private static Dictionary<string, int> WordCount(string words)
    {
        var allWords = Regex.Matches(words.ToLower(), @"\b\w+\b")
            .Select(m => m.Value);
        
        var wordCounts = allWords.GroupBy(g => g)
            .ToDictionary(x => x.Key, x => x.Count());

        return wordCounts.OrderByDescending(o => o.Value)
            .ToDictionary();
    }
}