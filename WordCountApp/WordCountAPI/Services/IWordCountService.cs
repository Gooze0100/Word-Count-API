using CSharpFunctionalExtensions;
using WordCountAPI.Models;

namespace WordCountAPI.Services;

public interface IWordCountService
{
    Task<Result<List<EachWordOccurrences>, Exception>> LoadFile(IFormFileCollection files, CancellationToken cancellationToken);
}