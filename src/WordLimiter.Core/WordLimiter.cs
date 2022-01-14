using Microsoft.Extensions.Logging;
using WordLimiter.Core.Dependencies;
using WordLimiter.Core.Entities;

namespace WordLimiter.Core;

public class WordLimiter : IWordLimiter
{
    private readonly ILogger _logger;
    private readonly IWordRepository _wordRepository;

    public WordLimiter(
        ILogger<WordLimiter> logger,
        IWordRepository wordRepository)
    {
        _logger = logger;
        _wordRepository = wordRepository;
    }

    public Output SuggestWords(Input input)
    {
        var words = _wordRepository
            .GetWords(input.Length)
            .Select(word => word.ToLower())
            .Distinct();

        foreach(var guess in input.Guesses)
            words = LimitWordsByGuess(words, guess);

        var output = new Output(words);
        return output;
    }

    private IEnumerable<string> LimitWordsByGuess(IEnumerable<string> words, Guess guess)
    {
        var lettersToInclude = guess
            .GuessLetters
            .Where(gl => gl.Status == GuessLetterStatus.Correct || gl.Status == GuessLetterStatus.WrongPlace)
            .Select(gl => gl.Letter);
        words = KeepWordsContainingLetters(words, lettersToInclude);

        var lettersToExclude = guess
            .GuessLetters
            .Where(gl => gl.Status == GuessLetterStatus.Invalid)
            .Select(gl => gl.Letter)
            .Where(c => !lettersToInclude.Contains(c)); // Exclude letters to include
        words = RemoveWordsContainingLetters(words, lettersToExclude);

        var lettersToIncludeAtIndex = guess
            .GuessLetters
            .Where(gl => gl.Status == GuessLetterStatus.Correct)
            .Select(gl => (gl.Index, gl.Letter));
        words = KeepWordsContainingLettersAtIndex(words, lettersToIncludeAtIndex);

        var lettersToExcludeAtIndex = guess
            .GuessLetters
            .Where(gl => gl.Status == GuessLetterStatus.WrongPlace)
            .Select(gl => (gl.Index, gl.Letter));
        words = RemoveWordsContainingLettersAtIndex(words, lettersToExcludeAtIndex);

        return words;
    }
    private IEnumerable<string> KeepWordsContainingLetters(IEnumerable<string> words, IEnumerable<char> letters)
    {
        var validWords = new List<string>();

        foreach(var word in words)
        {
            var wordChars = new List<char>(word);
            var lettersRemaining = new List<char>(letters);

            foreach(var letter in letters)
            {
                if(wordChars.Contains(letter))
                {
                    wordChars.Remove(letter);
                    lettersRemaining.Remove(letter);
                }
            }

            if(lettersRemaining.Count == 0)
                validWords.Add(word);
        }

        return validWords;
    }

    private IEnumerable<string> RemoveWordsContainingLetters(IEnumerable<string> words, IEnumerable<char> letters)
    {
        words = words.Distinct(); // Limit redundancy

        var validWords = new List<string>();
        foreach(var word in words)
        {
            var wordValid = true;
            foreach(var letter in letters)
            {
                if(word.Contains(letter))
                {
                    wordValid = false;
                    break;
                }
            }

            if(wordValid)
                validWords.Add(word);
        }
        return validWords;
    }

    private IEnumerable<string> KeepWordsContainingLettersAtIndex(IEnumerable<string> words, IEnumerable<(int, char)> lettersWithIndex)
    {
        var validWords = new List<string>();

        foreach(var word in words)
        {
            var letterAtIndexMatch = 0;
            
            foreach(var letterWithIndex in lettersWithIndex)
            {
                if(word[letterWithIndex.Item1] == letterWithIndex.Item2)
                    letterAtIndexMatch++;
            }

            if(letterAtIndexMatch == lettersWithIndex.Count())
                validWords.Add(word);
        }

        return validWords;
    }

    private IEnumerable<string> RemoveWordsContainingLettersAtIndex(IEnumerable<string> words, IEnumerable<(int, char)> lettersWithIndex)
    {
        var validWords = new List<string>();

        foreach(var word in words)
        {
            var wordValid = true;

            foreach(var letterWithIndex in lettersWithIndex)
            {
                if(word[letterWithIndex.Item1] == letterWithIndex.Item2)
                {
                    wordValid = false;
                    break;
                }
            }

            if(wordValid)
                validWords.Add(word);
        }

        return validWords;
    }
}