using Api.Data;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Api.Services.PaycheckCalculators;
using Api.Validation;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public interface IEmployeeService
    {
        /// <summary>
        /// Get all employees
        /// </summary>
        /// <returns>List of GetEmployeeDto objects</returns>
        List<GetEmployeeDto> GetAll();

        /// <summary>
        /// Get employee by id
        /// </summary>
        /// <param name="id">Id of the employee to get</param>
        /// <returns>The employee, or null if they dont exist</returns>
        GetEmployeeDto Get(int id);

        /// <summary>
        /// Calculates and returns the employees paycheck
        /// </summary>
        /// <param name="employeeId">Id of the employee</param>
        /// <returns>A GetPaycheckDto object</returns>
        GetPaycheckDto GetPaycheck(int employeeId);

        /// <summary>
        /// Creates an employee
        /// </summary>
        /// <param name="employee"><see cref="CreateEmployeeDto"/> employee to create</param>
        /// <returns><see cref="GetEmployeeDto"/> of the created employee</returns>
        GetEmployeeDto Create(CreateEmployeeDto employee);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly BenefitsDbContext _context;
        private readonly IEmployeeValidator _employeeValidator;
        private readonly IPaycheckCalculator _paycheckCalculator;

        public EmployeeService(
            BenefitsDbContext context, 
            IEmployeeValidator employeeValidator, 
            IPaycheckCalculator paycheckCalculator
            )
        {
            _context = context;
            _employeeValidator = employeeValidator;
            _paycheckCalculator = paycheckCalculator;
        }

        public List<GetEmployeeDto> GetAll()
        {
            var employees = _context.Employees
                .Include(e => e.Dependents)
                .Select(MapToDto).ToList();

            return employees;
        }

        public GetEmployeeDto Get(int id)
        {
            var employee = _context.Employees
                .Include(e => e.Dependents)
                .Select(MapToDto).FirstOrDefault(e => e.Id == id);

            // could be null, but lets handle in the controller
            return employee;
        }

        public GetPaycheckDto GetPaycheck(int employeeId) {
            var employee = Get(employeeId);
            return _paycheckCalculator.CalculatePaycheck(employee);
        }

        private GetEmployeeDto MapToDto(Employee employee)
        {
            return new GetEmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Salary = employee.Salary,
                DateOfBirth = employee.DateOfBirth,
                Dependents = employee.Dependents.Select(d => new GetDependentDto
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Relationship = d.Relationship,
                    DateOfBirth = d.DateOfBirth
                }).ToList()
            };
        }
    
        public GetEmployeeDto Create(CreateEmployeeDto employee)
        {

            var validationResult = _employeeValidator.Validate(employee);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.ErrorMessage ?? "Invalid employee");
            }

            var id = _context.Employees.Max(e => e.Id) + 1; // bit of a hack to get an auto-increment, generally the db would handle this
            
            var newEmployee = new Employee
            {
                Id = id, 
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Salary = employee.Salary,
                DateOfBirth = employee.DateOfBirth,
                Dependents = employee.Dependents.Select((d, index) => new Dependent
                {
                    Id = _context.Dependents.Max(d => d.Id) + 1 + index, // haaaack
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Relationship = d.Relationship,
                    DateOfBirth = d.DateOfBirth,
                    EmployeeId = id
                }).ToList()
            };

            _context.Employees.Add(newEmployee);
            _context.Dependents.AddRange(newEmployee.Dependents);
            _context.SaveChanges();

            return Get(newEmployee.Id);
        }
    }
}