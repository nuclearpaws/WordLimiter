namespace WordLimiter.Core.Services;

public interface IWordScoreService
{
    int ScoreWord(string word);
}

public class WordScoreService
    : IWordScoreService
{
    private readonly IEnumerable<char> _vowels;
    private readonly IEnumerable<char> _commonLetters;
    private readonly IEnumerable<char> _commonFirstLetters;
    private readonly IEnumerable<char> _commonLastLetters;
    private readonly int _vowelScore;
    private readonly int _commonLetterScore;
    private readonly int _commonFirstLetterScore;
    private readonly int _commonLastLetterScore;
    private readonly int _duplicateLetterScore;

    public WordScoreService(
        IEnumerable<char> vowels,
        IEnumerable<char> commonLetters,
        IEnumerable<char> commonFirstLetters,
        IEnumerable<char> commonLastLetters,
        int vowelScore,
        int commonLetterScore,
        int commonFirstLetterScore,
        int commonLastLetterScore,
        int duplicateLetterScore
    )
    {
        _vowels = vowels;
        _commonLetters = commonLetters;
        _commonFirstLetters = commonFirstLetters;
        _commonLastLetters = commonLastLetters;
        _vowelScore = vowelScore;
        _commonLetterScore = commonLetterScore;
        _commonFirstLetterScore = commonFirstLetterScore;
        _commonLastLetterScore = commonLastLetterScore;
        _duplicateLetterScore = duplicateLetterScore;
    }

    public int ScoreWord(string word)
    {
        var score = 0;

        // Vowels:
        score += word.Count(c => _vowels.Contains(c)) * _vowelScore;
        
        // Common Letters:
        score += word.Count(c => _commonLetters.Contains(c)) * _commonLetterScore;
        
        // Common First Letters:
        if(_commonFirstLetters.Contains(word.First()))
            score += _commonFirstLetterScore;
        
        // Common Last Letters:
        if(_commonLastLetters.Contains(word.Last()))
            score += _commonLastLetterScore;

        // Duplicates:
        var duplicates = word
            .GroupBy(c => c)
            .Count(gbr => gbr.Count() > 1);
        if(duplicates > 0)
            score += duplicates * _duplicateLetterScore;

        return score;
    }
}