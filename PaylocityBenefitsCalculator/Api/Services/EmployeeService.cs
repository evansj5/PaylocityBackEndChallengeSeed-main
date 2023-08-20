using Api.Data;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Api.Services
{
    public interface IEmployeeService
    {
        List<GetEmployeeDto> GetAll();
        GetEmployeeDto Get(int id);
        GetPaycheckDto GetPaycheck(int employeeId);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly BenefitsDbContext _context;
        private readonly IOptions<BenefitsCostSettings> _benefitsCostSettings;

        public EmployeeService(BenefitsDbContext context, IOptions<BenefitsCostSettings> benefitsCostSettings)
        {
            _context = context;
            _benefitsCostSettings = benefitsCostSettings;
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

            /**
                *Completely* unsure how fractional cents should be handled in these types of systems. I am going to just lop off the fractional cents.
                I assume in a real HR system there would be some logic that trues that up throughout the year.
            **/
            var employee = Get(employeeId);
            var grossPay = FloorToNearestCent(ConvertYearlyAmountToPaycheckAmount(employee.Salary));
            var benefitsCosts = FloorToNearestCent(ConvertMonthlyAmountToPaycheckAmount(_benefitsCostSettings.Value.EmployeeCostPerMonth));
            var dependentCosts = FloorToNearestCent(employee.Dependents
                .Sum(CalculatePaycheckDependentCost));

            var age = CalculateAge(employee.DateOfBirth);
            decimal ageBasedCosts = 0;

            if (age > _benefitsCostSettings.Value.AgeBasedCosts?.AgeThreshold)
            {
                ageBasedCosts = FloorToNearestCent(ConvertMonthlyAmountToPaycheckAmount(_benefitsCostSettings.Value.AgeBasedCosts?.AdditionalMonthlyCost ?? 0));
            }

            decimal salaryBasedCosts = 0;

            if (employee.Salary > _benefitsCostSettings.Value.SalaryBasedCosts?.SalaryThreshold)
            {
                var yearlyCost = (_benefitsCostSettings.Value.SalaryBasedCosts?.AdditionalYearlyCostPercent ?? 0) * employee.Salary;
                salaryBasedCosts = FloorToNearestCent(ConvertYearlyAmountToPaycheckAmount(yearlyCost));
            }

            return new GetPaycheckDto
            {
                GrossPay = grossPay,
                DependentsBenefitsCosts = dependentCosts,
                BaseBenefitsCosts = benefitsCosts,
                SalaryBenefitsCosts = salaryBasedCosts,
                AgeBasedBenefitsCosts = ageBasedCosts,
                NetPay = grossPay - benefitsCosts - dependentCosts - salaryBasedCosts - ageBasedCosts
            };
        }

        private decimal CalculatePaycheckDependentCost(GetDependentDto dependent)
        {
            var age = CalculateAge(dependent.DateOfBirth);
            var cost = _benefitsCostSettings.Value.DependentCostPerMonth;
            if (age > _benefitsCostSettings.Value.AgeBasedCosts?.DependentAgeThreshold)
            {
                cost += _benefitsCostSettings.Value.AgeBasedCosts?.DependentAdditionalMonthlyCost ?? 0;
            }
            return ConvertMonthlyAmountToPaycheckAmount(cost);
        }

        private decimal ConvertMonthlyAmountToPaycheckAmount(decimal monthlyAmount)
        {
            return monthlyAmount * 12 / _benefitsCostSettings.Value.PaychecksPerYear;
        }

        private decimal ConvertYearlyAmountToPaycheckAmount(decimal yearlyAmount)
        {
            return yearlyAmount / _benefitsCostSettings.Value.PaychecksPerYear;
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;

            return age;
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

        private decimal FloorToNearestCent(decimal value) 
        {
            return Math.Floor(value * 100) / 100;
        }
    }
}