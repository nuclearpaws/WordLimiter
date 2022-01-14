namespace WordLimiter.Core.Dependencies;

public interface IWordRepository
{
    IEnumerable<string> GetWords(int length);
}