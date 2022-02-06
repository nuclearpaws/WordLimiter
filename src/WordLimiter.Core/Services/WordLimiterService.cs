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
            if(!WordPassesLetterOccurenceSpecification(
                word,
                specification.Letter,
                specification.MinOccurances,
                specification.MaxOccurances))
                continue;

            if(!WordPassesRequiredIndeciesSpecification(
                word,
                specification.Letter,
                specification.RequiredIndecies))
                continue;

            if(!WordPassesAllowedIndeciesSpecification(
                word,
                specification.Letter,
                specification.AllowedIndecies))
                continue;

            passingWords.Add(word);
        }
        return passingWords;
    }

    private static bool WordPassesLetterOccurenceSpecification(string word, char letter, int min, int? max)
    {
        var occurances = word.Count(c => c == letter);

        if(occurances < min)
            return false;

        if(max != null && occurances > max)
            return false;

        return true;
    }

    private static bool WordPassesRequiredIndeciesSpecification(string word, char letter, IEnumerable<int> requiredIndecies)
    {
        foreach(var index in requiredIndecies)
        {
            if(word[index] != letter)
                return false;
        }

        return true;
    }

    private static bool WordPassesAllowedIndeciesSpecification(string word, char letter, IEnumerable<int> allowedIndecies)
    {
        if(!allowedIndecies.Any() && word.Contains(letter))
            return false;

        for(var i = 0; i < word.Length; i++)
        {
            if(word[i] == letter && !allowedIndecies.Contains(i))
                return false;
        }

        return true;
    }
}