using Api.Dtos.Employee;
using Api.Models;

namespace Api.Validation
{
    public interface IDependentValidator
    {
        ValidationResult Validate(CreateDependentDto dependent, Employee? employee);
    }

    public class DependentValidator : IDependentValidator
    {
        public ValidationResult Validate(CreateDependentDto dependent, Employee? employee)
        {
            
            if (string.IsNullOrWhiteSpace(dependent.FirstName))
            {
                return ValidationResult.Failure("First name is required");
            }

            if (string.IsNullOrWhiteSpace(dependent.LastName))
            {
                return ValidationResult.Failure("Last name is required");
            }

            if (dependent.DateOfBirth == default)
            {
                return ValidationResult.Failure("Date of birth is required");
            }

            if (employee == null)
            {
                throw new ValidationException($"Employee with id {dependent.EmployeeId} not found");
            }

            if (dependent.Relationship == Relationship.DomesticPartner || dependent.Relationship == Relationship.Spouse)
            {
                if (employee.Dependents.Any(d => d.Relationship == Relationship.DomesticPartner || d.Relationship == Relationship.Spouse))
                {
                    throw new ValidationException("Cannot add another spouse or domestic partner");
                }
            }

            return ValidationResult.Success();
        }
    }
}