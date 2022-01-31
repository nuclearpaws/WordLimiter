namespace WordLimiter.Core.Exceptions;

public class InvalidRequestException
    : Exception
{
    public IDictionary<string, IEnumerable<string>> ValidationErrors { get; init; }

    public InvalidRequestException(IDictionary<string, IEnumerable<string>> validationErrors)
        : base($"Invalid Request Recieved. See '{nameof(ValidationErrors)}' for more info.")
    {
        ValidationErrors = validationErrors;
    }
}