using Cocoa.Configuration;

namespace Cocoa.Validation;

public interface IValidation
{
    IReadOnlyCollection<ValidationResult> Validate(ChocolateyConfiguration configuration);
}