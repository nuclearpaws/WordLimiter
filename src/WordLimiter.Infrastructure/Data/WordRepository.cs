using WordLimiter.Core.Dependencies;

namespace WordLimiter.Infrastructure.Data;

public class WordRepository
    : IWordRepository
{
    public IEnumerable<string> GetWords(int length)
    {
        var words = GetAllWords();
        words = SanitiseWords(length, words);
        return words;
    }

    public async Task<IEnumerable<string>> GetWordsAsync(int length, CancellationToken cancellationToken = default)
    {
        var words = await GetAllWordsAsync(cancellationToken);
        words = SanitiseWords(length, words);
        return words;
    }

    private IEnumerable<string> GetAllWords()
    {
        var words = new List<string>();

        var fileNames = GetFiles();
        foreach(var fileName in fileNames)
        {
            var fileWords = ReadFile(fileName);
            words.AddRange(fileWords);
        }

        return words;
    }

    private async Task<IEnumerable<string>> GetAllWordsAsync(CancellationToken cancellationToken = default)
    {
        var words = new List<string>();

        var fileNames = GetFiles();
        foreach(var fileName in fileNames)
        {
            var fileWords = await ReadFileAsync(fileName, cancellationToken);
            words.AddRange(fileWords);
        }

        return words;
    }

    private IEnumerable<string> ReadFile(string fileName)
    {
        var content = string.Empty;
        using(var reader = new StreamReader(fileName))
        {
            content = reader.ReadToEnd();
        }

        var lines = content.Split(new char[] { '\r', '\n' });
        return lines;
    }

    private async Task<IEnumerable<string>> ReadFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var content = string.Empty;
        using(var reader = new StreamReader(fileName))
        {
            content = await reader.ReadToEndAsync();
        }

        var lines = content.Split(new char[] { '\r', '\n' });
        return lines;
    }

    private IEnumerable<string> GetFiles()
    {
        var fileNames = Directory.GetFiles("./Words");
        return fileNames;
    }

    private IEnumerable<string> SanitiseWords(int length, IEnumerable<string> words)
    {
        var output = words
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(word => word.Trim())
            .Where(word => word.Length == length)
            .Select(word => word.ToLower())
            .ToList();

        return output;
    }
}