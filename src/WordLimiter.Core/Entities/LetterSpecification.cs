using WordLimiter.Core.Helpers;

namespace WordLimiter.Core.Entities;

public record LetterSpecification
{
    public char Letter { get; set; }
    public int ConfirmedCount { get; set; }
    public IDictionary<int, LetterSpecificationStatus> Indecies { get; set; }

    public LetterSpecification()
    {
        Letter = default;
        ConfirmedCount = 0;
        Indecies = new Dictionary<int, LetterSpecificationStatus>();
    }

    public static IEnumerable<LetterSpecification> GetAllAllowed()
    {
        var letterSpecifications = LetterHelper
            .GetAllAToZLowerLetters()
            .Select(c => new LetterSpecification
            {
                Letter = c,
            })
            .ToList();
        return letterSpecifications;
    }

    public enum LetterSpecificationStatus
    {
        Required = 0,
        Forbidden = 1,
    }
}