using System.ComponentModel.DataAnnotations;

namespace Services.Helper
{
    public class ValidationHelper
    {

        internal static void ModelValidation(object obj)
        {
            ValidationContext validation = new ValidationContext(obj);

            List<ValidationResult> errors = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validation, errors, true);

            if (!isValid)
            {
                //foreach (var err in errors)
                //{
                //    throw new ArgumentException(err.ErrorMessage);
                //}
                string errorMessage = string.Join(" ", errors.Select(temp => temp?.ErrorMessage));
                throw new ArgumentException(errorMessage);

            }
        }
    }
}
