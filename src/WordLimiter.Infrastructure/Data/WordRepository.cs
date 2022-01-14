using WordLimiter.Core.Dependencies;

namespace WordLimiter.Infrastructure.Data;

public class WordRepository
    : IWordRepository
{
    public IEnumerable<string> GetWords(int length)
    {
        var words = GetAllWords();

        words = words
            .Select(word => word.Trim())
            .Where(word => word.Length == length)
            .Select(word => word.ToLower())
            .ToList();

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

    private IEnumerable<string> ReadFile(string fileName)
    {
        var content = string.Empty;
        using(var reader = new StreamReader(fileName))
        {
            content = reader.ReadToEnd();
        }

        IEnumerable<string> lines = content.Split(new char[] { '\r', '\n' });
        lines = lines
            .Distinct()
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        return lines;
    }

    private IEnumerable<string> GetFiles()
    {
        var fileNames = Directory.GetFiles("./Words");
        return fileNames;
    }
}