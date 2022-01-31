using CommandLine;

namespace WordLimiter.Cli.ArgumentOptions;

internal sealed class DefaultArgs
{
    [Option('l', "length", Required = true)]
    public int Length { get; set; }

    [Option('g', "guess", Required = false)]
    public IEnumerable<string> Guesses { get; set; }

    public DefaultArgs()
    {
        Guesses = new List<string>();
    }
}