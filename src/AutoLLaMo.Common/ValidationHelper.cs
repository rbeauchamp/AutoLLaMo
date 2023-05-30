using System.ComponentModel.DataAnnotations;

namespace AutoLLaMo.Common
{
    /// <summary>
    /// Helper class for validating objects.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Validates the specified object and returns the validation results.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <returns>A tuple with a boolean indicating the validation result and a list of ValidationResults.</returns>
        public static (bool IsValid, List<ValidationResult> ValidationResults) Validate(this object objectToValidate)
        {
            var context = new ValidationContext(objectToValidate, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(objectToValidate, context, validationResults, validateAllProperties: true);

            return (isValid, validationResults);
        }
    }
}
