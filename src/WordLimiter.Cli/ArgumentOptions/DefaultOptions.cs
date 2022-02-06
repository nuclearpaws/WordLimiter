using CommandLine;

namespace WordLimiter.Cli.ArgumentOptions;

internal sealed class DefaultArgs
{
    [Option('l', "length", Required = true, HelpText = "The length of words to be limited by this tool (eg. 5).")]
    public int Length { get; set; }

    [Option('g', "guess", Required = false, HelpText = "The Wordle like guess represented like \"<word> <feedback>\" where word is the word guess and feedback is \".\" for invalid letter, \"?\" for misplaced letter and \"!\" for correctly guessed letter. Multiples are accepted.")]
    public IEnumerable<string> Guesses { get; set; }

    public DefaultArgs()
    {
        Guesses = new List<string>();
    }
}