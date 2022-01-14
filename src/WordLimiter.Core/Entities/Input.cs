namespace WordLimiter.Core.Entities;

public record Input(int Length, IEnumerable<Guess> Guesses);