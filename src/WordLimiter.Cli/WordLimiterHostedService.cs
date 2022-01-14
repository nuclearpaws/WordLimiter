using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using WordLimiter.Core;
using WordLimiter.Core.Entities;

namespace WordLimiter;

internal class WordLimiterHostedService
    : IHostedService
{
    private readonly ILogger _logger;
    private readonly IWordLimiter _wordLimiter;
    private readonly IHostApplicationLifetime _application;

    public WordLimiterHostedService(
        ILogger<WordLimiterHostedService> logger,
        IWordLimiter wordLimiter,
        IHostApplicationLifetime application)
    {
        _logger = logger;
        _wordLimiter = wordLimiter;
        _application = application;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var doSomethingTask = BackgroundWorker();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task BackgroundWorker()
    {
        var guesses = new List<Guess>();
        var words = string.Empty;

        guesses.Add(new Guess(new GuessLetter[]{
            new GuessLetter(0, 'l', GuessLetterStatus.Invalid),
            new GuessLetter(1, 'o', GuessLetterStatus.Invalid),
            new GuessLetter(2, 's', GuessLetterStatus.Invalid),
            new GuessLetter(3, 'e', GuessLetterStatus.Invalid),
            new GuessLetter(4, 'r', GuessLetterStatus.Invalid),
        }));

        guesses.Add(new Guess(new GuessLetter[]{
            new GuessLetter(0, 'q', GuessLetterStatus.Invalid),
            new GuessLetter(1, 'u', GuessLetterStatus.Invalid),
            new GuessLetter(2, 'i', GuessLetterStatus.Invalid),
            new GuessLetter(3, 'c', GuessLetterStatus.Invalid),
            new GuessLetter(4, 'k', GuessLetterStatus.Invalid),
        }));

        words = Guess(new Input(5, guesses));
        _logger.LogInformation($"Guess {guesses.Count}:\n{words}");

        _application.StopApplication();
        return Task.CompletedTask;
    }

    private string Guess(Input input)
    {
        var output = _wordLimiter.SuggestWords(input);

        var stringBuilder = new StringBuilder();
        foreach(var word in output.WordSuggestions)
            stringBuilder.AppendLine($"- {word}");

        return stringBuilder.ToString();
    }
}