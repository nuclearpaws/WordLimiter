namespace WordLimiter.Core.Entities;

public record GuessLetter(
    int Index,
    char Letter,
    GuessLetterStatus Status);