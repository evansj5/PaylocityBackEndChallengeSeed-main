using Api.Data;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public interface IEmployeeService
    {
        List<GetEmployeeDto> GetAll();
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
                .Select(e => new GetEmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Salary = e.Salary,
                    DateOfBirth = e.DateOfBirth,
                    Dependents = e.Dependents.Select(d => new GetDependentDto
                    {
                        Id = d.Id,
                        FirstName = d.FirstName,
                        LastName = d.LastName,
                        Relationship = d.Relationship,
                        DateOfBirth = d.DateOfBirth
                    }).ToList()
                }).ToList();

            return employees;
        }
    }
}