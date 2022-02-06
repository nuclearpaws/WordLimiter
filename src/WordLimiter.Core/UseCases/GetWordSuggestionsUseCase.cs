using FluentValidation;
using MediatR;
using WordLimiter.Core.Dependencies;
using WordLimiter.Core.Entities;
using WordLimiter.Core.Services;

namespace WordLimiter.Core.UseCases;

public sealed class GetWordSuggestionsUseCase
    : IRequestHandler<
        GetWordSuggestionsUseCase.Request,
        GetWordSuggestionsUseCase.Response>
{
    private readonly IWordRepository _wordRepository;
    private readonly IWordLimiterService _wordLimiterService;

    public GetWordSuggestionsUseCase(
        IWordRepository wordRepository,
        IWordLimiterService wordLimiterService)
    {
        _wordRepository = wordRepository;
        _wordLimiterService = wordLimiterService;
    }

    public async Task<Response> Handle(
        Request request,
        CancellationToken cancellationToken)
    {
        var words = await _wordRepository
            .GetWordsAsync(request.WordLength, cancellationToken);

        var letterSpecifications = GetLetterSpecificationsFromRequest(request);
        words = _wordLimiterService.LimitWords(words, letterSpecifications);

        // foreach(var guess in request.Guesses)
        //     words = LimitWordsByGuess(words, guess);

        var response = new Response
        {
            GuessCount = request.Guesses.Count(),
            SuggestionCount = words.Count(),
            Suggestions = words,
        };
        return response;
    }

    private static IEnumerable<LetterSpecification> GetLetterSpecificationsFromRequest(Request request)
    {
        var letterSpecifications = LetterSpecification.GetAllAllowed(request.WordLength);

        foreach(var guess in request.Guesses)
            letterSpecifications = LimitLetterSpecificationsByGuess(letterSpecifications, guess);

        return letterSpecifications;
    }

    private static IEnumerable<LetterSpecification> LimitLetterSpecificationsByGuess(IEnumerable<LetterSpecification> letterSpecifications, Request.GuessDto guess)
    {
        foreach(var guessLetter in guess.GuessLetters)
        {
            var letterSpecification = letterSpecifications.FirstOrDefault(ls => ls.Letter == guessLetter.Letter);
            
            if(letterSpecification == null)
                continue;
            
            switch(guessLetter.Status)
            {
                case Request.GuessDto.GuessLetterDto.GuessLetterStatusEnum.Correct:
                    var newRequiredIndecies = new List<int>(letterSpecification.RequiredIndecies);
                    newRequiredIndecies.Add(guessLetter.Index);
                    letterSpecification.RequiredIndecies = newRequiredIndecies;
                    break;
                case Request.GuessDto.GuessLetterDto.GuessLetterStatusEnum.Misplaced:
                    var newAllowedIndecies = new List<int>(letterSpecification.AllowedIndecies);
                    newAllowedIndecies.Remove(guessLetter.Index);
                    letterSpecification.AllowedIndecies = newAllowedIndecies;
                    break;
                case Request.GuessDto.GuessLetterDto.GuessLetterStatusEnum.Wrong:
                    letterSpecification.AllowedIndecies = letterSpecification.RequiredIndecies;
                    break;
                default:
                    break;
            }
        }

        return letterSpecifications;
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