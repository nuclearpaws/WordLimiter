using WordLimiter.Core.Entities;

namespace WordLimiter.Core;

public interface IWordLimiter
{
    Output SuggestWords(Input input);
}