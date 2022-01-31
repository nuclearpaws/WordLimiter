namespace WordLimiter.Core.Dependencies;

public interface IWordRepository
{
    IEnumerable<string> GetWords(int length);
    Task<IEnumerable<string>> GetWordsAsync(int length, CancellationToken cancellationToken = default);
}