using Newtonsoft.Json;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using WordLimiter.Cli.ArgumentOptions;
using WordLimiter.Core.UseCases;
using WordLimiter.Core.Exceptions;
using WordLimiter.Cli.Helpers;

namespace WordLimiter;

internal class WordLimiterHostedService
    : BackgroundService
{
    private readonly DefaultArgs _args;
    private readonly ILogger _logger;
    private readonly IMediator _mediator;
    private readonly IHostApplicationLifetime _application;

    public WordLimiterHostedService(
        DefaultArgs args,
        ILogger<WordLimiterHostedService> logger,
        IMediator mediator,
        IHostApplicationLifetime application)
    {
        _args = args;
        _logger = logger;
        _mediator = mediator;
        _application = application;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var request = new GetWordSuggestionsUseCase.Request
            {
                WordLength = _args.Length,
                Guesses = _args
                    .Guesses
                    .Select(guess => StringToGuess(_args.Length, guess))
                    .ToList(),
            };

            var response = await _mediator.Send(request, stoppingToken);
            var json = JsonConvert.SerializeObject(response, Formatting.Indented);

            if(string.IsNullOrWhiteSpace(_args.OutputFile))
            {
                _logger.LogInformation(json);
            }
            else
            {
                var outputFilePath = $"./{_args.OutputFile}.json";
                FileWriter.WriteFile(outputFilePath, json);
                _logger.LogInformation(outputFilePath);
            }
        }
        catch(InvalidRequestException ex)
        {
            var json = JsonConvert.SerializeObject(ex, Formatting.Indented);
            _logger.LogWarning(json);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "There was an unexpected exception:");
        }
        finally
        {
            _application.StopApplication();
        }
    }

    private static GetWordSuggestionsUseCase.Request.GuessDto StringToGuess(int length, string input)
    {
        Debug.Assert(input.Length == length * 2 + 1);

        var split = input.Split(new char[] { ' ' });
        var guessLetters = new List<GetWordSuggestionsUseCase.Request.GuessDto.GuessLetterDto>();
        for(var i = 0; i < length; i++)
        {
            guessLetters.Add(new GetWordSuggestionsUseCase.Request.GuessDto.GuessLetterDto
            {
                Index = i,
                Letter = split[0][i],
                Status = GetStatusFromChar(split[1][i]),
            });
        }
        return new GetWordSuggestionsUseCase.Request.GuessDto
        {
            GuessLetters = guessLetters,
        };
    }

    private static GetWordSuggestionsUseCase.Request.GuessDto.GuessLetterDto.GuessLetterStatusEnum GetStatusFromChar(char c)
    {
        switch(c)
        {
            case '.':
                return GetWordSuggestionsUseCase.Request.GuessDto.GuessLetterDto.GuessLetterStatusEnum.Wrong;
            case '?':
                return GetWordSuggestionsUseCase.Request.GuessDto.GuessLetterDto.GuessLetterStatusEnum.Misplaced;
            case '!':
                return GetWordSuggestionsUseCase.Request.GuessDto.GuessLetterDto.GuessLetterStatusEnum.Correct;
            default:
                throw new ArgumentOutOfRangeException(nameof(c));
        }
    }
}