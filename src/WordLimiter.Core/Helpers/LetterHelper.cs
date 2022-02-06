namespace WordLimiter.Core.Helpers;

internal static class LetterHelper
{
    public static IEnumerable<char> GetAllAToZLowerLetters()
    {
        var letters = Enumerable
            .Range((int)'a', (int)'z'-(int)'a')
            .Select(i => (char)i)
            .ToList();
        return letters;
    }
}