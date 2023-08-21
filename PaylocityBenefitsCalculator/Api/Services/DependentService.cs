using Api.Data;
using Api.Dtos.Dependent;
using Api.Models;
using Api.Validation;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public interface IDependentService
    {
        /// <summary>
        /// Get all dependents
        /// </summary>
        /// <returns>List of <see cref="GetDependentDto"/></returns>
        List<GetDependentDto> GetAll();
        /// <summary>
        /// Get dependent by id
        /// </summary>
        /// <param name="id">Id of the dependent</param>
        /// <returns><see cref="GetDependentDto"/> or null if the dependent doesn't exist</returns>
        GetDependentDto Get(int id);
        /// <summary>
        /// Creates a dependent
        /// </summary>
        /// <param name="dependent"><see cref="CreateDependentDto"/> to create</param>
        /// <returns><see cref="GetDependentDto"/> of the created user</returns>
        GetDependentDto Create(CreateDependentDto dependent);
    }

    public class DependentService : IDependentService
    {
        private readonly BenefitsDbContext _context;
        private readonly IDependentValidator _dependentValidator;

        public DependentService(BenefitsDbContext context, IDependentValidator dependentValidator)
        {
            _context = context;
            _dependentValidator = dependentValidator;
        }

        public List<GetDependentDto> GetAll()
        {
            var dependents = _context.Dependents
                .Select(MapToDto).ToList();

            return dependents;
        }

        public GetDependentDto Get(int id) 
        {
            var dependent = _context.Dependents
                .Select(MapToDto).FirstOrDefault(d => d.Id == id);

            // could be null, but lets handle in the controller
            return dependent;
        }

        public GetDependentDto Create(CreateDependentDto dependent)
        {
            var employee = _context.Employees.Include(employee => employee.Dependents).FirstOrDefault(e => e.Id == dependent.EmployeeId);
            var validationResult = _dependentValidator.Validate(dependent, employee);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.ErrorMessage ?? "Invalid dependent");
            }

            var dependentEntity = new Dependent
            {
                Id = _context.Dependents.Max(d => d.Id) + 1,
                FirstName = dependent.FirstName,
                LastName = dependent.LastName,
                Relationship = dependent.Relationship,
                DateOfBirth = dependent.DateOfBirth,
                EmployeeId = dependent.EmployeeId
            };

            employee.Dependents.Add(dependentEntity);  
            _context.Employees.Update(employee); 
            _context.Dependents.Add(dependentEntity);
            _context.SaveChanges();

            return Get(dependentEntity.Id);
        }

        private GetDependentDto MapToDto(Dependent dependent)
        {
            return new GetDependentDto
            {
                Id = dependent.Id,
                FirstName = dependent.FirstName,
                LastName = dependent.LastName,
                Relationship = dependent.Relationship,
                DateOfBirth = dependent.DateOfBirth,
            };
        }
    }
}