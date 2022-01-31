using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WordLimiter.Core.Exceptions;

namespace WordLimiter.Core.Behaviours;

public sealed class ValidationBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger _logger;

    public ValidationBehaviour(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehaviour<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            var validationContext = new ValidationContext<TRequest>(request);
            var validationFailures = _validators
                .Select(v => v.Validate(validationContext))
                .SelectMany(vr => vr.Errors)
                .Where(vf => vf != null)
                .ToList();

            if (validationFailures.Any())
            {
                var validationErrors = validationFailures
                    .GroupBy(vf => vf.PropertyName)
                    .ToDictionary(
                        gbr => gbr.Key,
                        gbr => gbr
                            .Select(vf => vf.ErrorMessage)
                            .ToList()
                            .AsEnumerable());

                throw new InvalidRequestException(validationErrors);
            }

            return next();
        }
        catch(InvalidRequestException)
        {
            // Pass InvalidRequestException as normal:
            throw;
        }
        catch(Exception ex)
        {
            // Wrap any other exception into this to make trace easier:
            throw new ApplicationException($"There was a problem during validation inside '{nameof(ValidationBehaviour<TRequest, TResponse>)}'.", ex);
        }
    }
}