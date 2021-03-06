using WordLimiter.Core.Helpers;

namespace WordLimiter.Core.Entities;

public record LetterSpecification
{
    public char Letter { get; set; }

    private IEnumerable<int> _requiredIndecies;
    public IEnumerable<int> RequiredIndecies
    {
        get => _requiredIndecies;
        set
        {
            value = value.Distinct();

            if(value.Except(_allowedIndecies).Any())
                throw new InvalidOperationException($"'{nameof(RequiredIndecies)}' must be contained within '{nameof(AllowedIndecies)}'.");

            _requiredIndecies = value;
        }
    }
    
    private IEnumerable<int> _allowedIndecies;
    public IEnumerable<int> AllowedIndecies
    {
        get => _allowedIndecies;
        set
        {
            value = value.Distinct();

            if(_requiredIndecies.Except(value).Any())
                throw new InvalidOperationException($"'{nameof(AllowedIndecies)}' must contain all values from '{nameof(RequiredIndecies)}'.");

            _allowedIndecies = value;
        }
    }

    public int MinOccurances
    {
        get => _requiredIndecies.Count();
    }

    public int MaxOccurances
    {
        get => _allowedIndecies.Count();
    }

    public LetterSpecification()
    {
        _requiredIndecies = new List<int>();
        _allowedIndecies = new List<int>();
    }

    public static IEnumerable<LetterSpecification> GetAllAllowed(int wordLength)
    {
        var letterSpecifications = LetterHelper
            .GetAllAToZLowerLetters()
            .Select(c => new LetterSpecification
            {
                Letter = c,
                RequiredIndecies = new int[]{},
                AllowedIndecies = Enumerable.Range(0, wordLength),
            })
            .ToList();
        return letterSpecifications;
    }
}