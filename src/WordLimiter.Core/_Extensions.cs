using FluentValidation;

namespace WordLimiter.Core;

internal static class _Extensions
{
    public static IRuleBuilder<TRequest, IEnumerable<TProperty>> CollectionMustBeOfLength<TRequest, TProperty>(
        this IRuleBuilder<TRequest, IEnumerable<TProperty>> ruleBuilder,
        int length)
    {
        return ruleBuilder
            .Must(p => p.Count() == length)
            .WithMessage($"Collection must be '{length}' items long.");
    }

    public static IRuleBuilder<TRequest, char> OnlyAlphaCharacter<TRequest>(
        this IRuleBuilder<TRequest, char> ruleBuilder)
    {
        var allowedCharacters = new char[]
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'p', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        };
        return ruleBuilder
            .Must(p => allowedCharacters.Contains(p.ToString().ToLower()[0]))
            .WithMessage($"Must be a valid alpha character ({string.Join(", ", allowedCharacters)}).");
    }
}