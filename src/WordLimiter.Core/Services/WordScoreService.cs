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

    public WordScoreService()
    {
        _vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };
        _commonLetters = new char[] { 'e', 't', 'a', 'i', 'o', 'n', 's', 'h', 'r' };
        _commonFirstLetters = new char[] { 't', 'a', 'o', 'd', 'w' };
        _commonLastLetters = new char[] { 'e', 's', 'd', 't' };
    }

    public int ScoreWord(string word)
    {
        const int vowelScore = 3;
        const int commonLetterScore = 5;
        const int commonFirstLetterScore = 7;
        const int commonLastLetterScore = 7;
        const int duplicateLetterScore = -10;

        var score = 0;

        // Vowels:
        score += word.Count(c => _vowels.Contains(c)) * vowelScore;
        
        // Common Letters:
        score += word.Count(c => _commonLetters.Contains(c)) * commonLetterScore;
        
        // Common First Letters:
        if(_commonFirstLetters.Contains(word.First()))
            score += commonFirstLetterScore;
        
        // Common Last Letters:
        if(_commonLastLetters.Contains(word.Last()))
            score += commonLastLetterScore;

        // Duplicates:
        var duplicates = word
            .GroupBy(c => c)
            .Count(gbr => gbr.Count() > 1);
        if(duplicates > 0)
            score += duplicates * duplicateLetterScore;

        return score;
    }
}