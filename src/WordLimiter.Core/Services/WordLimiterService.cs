using WordLimiter.Core.Entities;

namespace WordLimiter.Core.Services;

public interface IWordLimiterService
{
    IEnumerable<string> LimitWords(
        IEnumerable<string> words,
        IEnumerable<LetterSpecification> letterSpecifications);
}

public class WordLimiterService
    : IWordLimiterService
{
    public IEnumerable<string> LimitWords(
        IEnumerable<string> words,
        IEnumerable<LetterSpecification> letterSpecifications)
    {
        var passingWords = words;
        foreach(var letterSpecification in letterSpecifications)
        {
            passingWords = LimitWordsByLetterSpecification(passingWords, letterSpecification);
        }
        return passingWords;
    }

    private static IEnumerable<string> LimitWordsByLetterSpecification(
        IEnumerable<string> words,
        LetterSpecification specification)
    {
        var passingWords = new List<string>();
        foreach(var word in words)
        {
            if(!WordPassesRequiredLetterCount(
                word,
                specification.Letter,
                specification.ConfirmedCount,
                int.MaxValue))
                continue;

            if(!WordPassesRequiredIndecies(
                word,
                specification.Letter,
                specification
                    .Indecies
                    .Where(e => e.Value == LetterSpecification.LetterSpecificationStatus.Required)
                    .Select(e => e.Key)))
                continue;

            if(!WordPassesForbiddenIndecies(
                word,
                specification.Letter,
                specification
                    .Indecies
                    .Where(e => e.Value == LetterSpecification.LetterSpecificationStatus.Forbidden)
                    .Select(e => e.Key)))
                continue;
            
            passingWords.Add(word);
        }
        return passingWords;
    }

    private static bool WordPassesRequiredLetterCount(
        string word,
        char letter,
        int minCount,
        int maxCount)
    {
        var wordLetterCount = word.Count(c => c == letter);

        if(wordLetterCount > maxCount)
            return false;

        if(wordLetterCount < minCount)
            return false;

        return true;
    }

    private static bool WordPassesRequiredIndecies(
        string word,
        char letter,
        IEnumerable<int> requiredIndecies)
    {
        foreach(var requiredIndex in requiredIndecies)
        {
            if(word[requiredIndex] != letter)
                return false;
        }

        return true;
    }

    private static bool WordPassesForbiddenIndecies(
        string word,
        char letter,
        IEnumerable<int> forbiddenIndecies)
    {
        foreach(var forbiddenIndex in forbiddenIndecies)
        {
            if(word[forbiddenIndex] == letter)
                return false;
        }

        return true;
    }
}