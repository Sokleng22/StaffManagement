using System.ComponentModel.DataAnnotations;

namespace StaffManagement.SharedLib.Attributes
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime <= DateTime.Today;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} cannot be a future date.";
        }
    }

    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        public override bool IsValid(object? value)
        {
            if (value is DateTime dateTime)
            {
                var age = DateTime.Today.Year - dateTime.Year;
                if (dateTime.Date > DateTime.Today.AddYears(-age))
                    age--;
                
                return age >= _minimumAge;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} indicates an age less than {_minimumAge} years.";
        }
    }

    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _dependentProperty;
        private readonly object _targetValue;

        public RequiredIfAttribute(string dependentProperty, object targetValue)
        {
            _dependentProperty = dependentProperty;
            _targetValue = targetValue;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_dependentProperty);
            if (property == null)
            {
                return new ValidationResult($"Unknown property: {_dependentProperty}");
            }

            var dependentValue = property.GetValue(validationContext.ObjectInstance);
            if (Equals(dependentValue, _targetValue))
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required.");
                }
            }

            return ValidationResult.Success;
        }
    }

    public class ValidDepartmentAttribute : ValidationAttribute
    {
        private static readonly string[] ValidDepartments = 
        {
            "Engineering", "Human Resources", "Finance", "Marketing", "Sales", 
            "Operations", "IT", "Legal", "Executive", "Customer Service", "Research", "Quality"
        };

        public override bool IsValid(object? value)
        {
            if (value is string department)
            {
                return ValidDepartments.Contains(department, StringComparer.OrdinalIgnoreCase);
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be one of the following: {string.Join(", ", ValidDepartments)}.";
        }
    }
}
