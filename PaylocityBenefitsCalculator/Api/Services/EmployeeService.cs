using Api.Data;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public interface IEmployeeService
    {
        List<GetEmployeeDto> GetAll();
        GetEmployeeDto Get(int id);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly BenefitsDbContext _context;

        public EmployeeService(BenefitsDbContext context)
        {
            _context = context;
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
    }
}