using Api.Models;

namespace Api.Validation
{
    public interface IEmployeeValidator
    {
        ValidationResult Validate(CreateEmployeeDto employee);
    }

    // this is possibly a little overengineered for such a small example, but I think it would be important to have a validation framework in place
    // especially for something that would be as complicated as an employee. I think this would grow.
    public class EmployeeValidator : IEmployeeValidator
    {
        public ValidationResult Validate(CreateEmployeeDto employee)
        {
            var spouseCount = employee.Dependents.Count(d => d.Relationship == Relationship.Spouse || d.Relationship == Relationship.DomesticPartner);

            if (spouseCount > 1) 
            {
                return ValidationResult.Failure("An employee may only have one spouse or domestic partner");
            }
            
            if (string.IsNullOrWhiteSpace(employee.FirstName))
            {
                return ValidationResult.Failure("First name is required");
            }

            if (string.IsNullOrWhiteSpace(employee.LastName))
            {
                return ValidationResult.Failure("Last name is required");
            }

            if (employee.Salary <= 0)
            {
                return ValidationResult.Failure("Salary must be greater than 0");
            }

            if (employee.DateOfBirth == default)
            {
                return ValidationResult.Failure("Date of birth is required");
            }

            return ValidationResult.Success();
        }
    }
}