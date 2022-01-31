using FluentValidation;
using MediatR;
using WordLimiter.Core.Dependencies;

namespace WordLimiter.Core.UseCases;

public sealed class GetWordSuggestionsUseCase
    : IRequestHandler<
        GetWordSuggestionsUseCase.Request,
        GetWordSuggestionsUseCase.Response>
{
    private readonly IWordRepository _wordRepository;

    public GetWordSuggestionsUseCase(IWordRepository wordRepository)
    {
        _wordRepository = wordRepository;
    }

    public async Task<Response> Handle(
        Request request,
        CancellationToken cancellationToken)
    {
        var words = await _wordRepository
            .GetWordsAsync(request.WordLength, cancellationToken);

        foreach(var guess in request.Guesses)
            words = LimitWordsByGuess(words, guess);

        var response = new Response
        {
            GuessCount = request.Guesses.Count(),
            SuggestionCount = words.Count(),
            Suggestions = words,
        };
        return response;
    }

    private IEnumerable<string> LimitWordsByGuess(IEnumerable<string> words, Request.GuessDto guess)
    {
        var lettersToInclude = guess
            .GuessLetters
            .Where(gl => gl.Status == Request.GuessDto.GuessLetterDto.GuessLetterStatusEnum.Correct || gl.Status == Request.GuessDto.GuessLetterDto.GuessLetterStatusEnum.Misplaced)
            .Select(gl => gl.Letter);
        words = KeepWordsContainingLetters(words, lettersToInclude);

        var lettersToExclude = guess
            .GuessLetters
            .Where(gl => gl.Status == Request.GuessDto.GuessLetterDto.GuessLetterStatusEnum.Wrong)
            .Select(gl => gl.Letter)
            .Where(c => !lettersToInclude.Contains(c)); // Exclude letters to include
        words = RemoveWordsContainingLetters(words, lettersToExclude);

        var lettersToIncludeAtIndex = guess
            .GuessLetters
            .Where(gl => gl.Status == Request.GuessDto.GuessLetterDto.GuessLetterStatusEnum.Correct)
            .Select(gl => (gl.Index, gl.Letter));
        words = KeepWordsContainingLettersAtIndex(words, lettersToIncludeAtIndex);

        var lettersToExcludeAtIndex = guess
            .GuessLetters
            .Where(gl => gl.Status == Request.GuessDto.GuessLetterDto.GuessLetterStatusEnum.Wrong)
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

    public sealed class Validator
        : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.WordLength)
                .GreaterThan(0);

            When(r => r.Guesses.Count() > 0, () => {
                RuleForEach(r => r.Guesses)
                    .SetValidator(r => new GuessDtoValidator(r.WordLength));
            });
        }

        public sealed class GuessDtoValidator
            : AbstractValidator<Request.GuessDto>
        {
            public GuessDtoValidator(int wordLength)
            {
                RuleFor(r => r.GuessLetters)
                    .CollectionMustBeOfLength(wordLength);

                RuleForEach(r => r.GuessLetters)
                    .SetValidator(r => new GuessLetterDtoValidator(wordLength));
            }

            public sealed class GuessLetterDtoValidator
                : AbstractValidator<Request.GuessDto.GuessLetterDto>
            {
                public GuessLetterDtoValidator(int wordLength)
                {
                    RuleFor(r => r.Index)
                        .InclusiveBetween(0, wordLength-1);

                    // RuleFor(r => r.Letter)
                    //     .OnlyAlphaCharacter();
                }
            }
        }
    }

    public sealed class Request
        : IRequest<Response>
    {
        public int WordLength { get; set; }
        public IEnumerable<GuessDto> Guesses { get; set; }

        public Request()
        {
            Guesses = new List<GuessDto>();
        }

        public sealed class GuessDto
        {
            public IEnumerable<GuessLetterDto> GuessLetters { get; set; }

            public GuessDto()
            {
                GuessLetters = new List<GuessLetterDto>();
            }

            public sealed class GuessLetterDto
            {
                public int Index { get; set; }
                public char Letter { get; set; }
                public GuessLetterStatusEnum Status { get; set; }

                public enum GuessLetterStatusEnum
                {
                    Wrong = 0,
                    Misplaced = 1,
                    Correct = 2,
                }
            }
        }
    }

    public sealed class Response
    {
        public int GuessCount { get; set; }
        public int SuggestionCount { get; set; }
        public IEnumerable<string> Suggestions { get; set; }

        public Response()
        {
            Suggestions = new List<string>();
        }
    }
}