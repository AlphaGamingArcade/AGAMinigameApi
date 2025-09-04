using System.ComponentModel.DataAnnotations;

namespace AGAMinigameApi.Validations;
public class MinimumAgeAttribute : ValidationAttribute
{
    private readonly int _minimumAge;

    public MinimumAgeAttribute(int minimumAge)
    {
        _minimumAge = minimumAge;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateOfBirth)
        {
            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
            {
                age--;
            }

            if (age < _minimumAge)
            {
                return new ValidationResult($"Minimum age of {_minimumAge} years is required.");
            }
        }
        // If the value is null, other attributes like [Required] should handle it.
        // Returning success here allows other validation attributes to run.
        return ValidationResult.Success;
    }
}