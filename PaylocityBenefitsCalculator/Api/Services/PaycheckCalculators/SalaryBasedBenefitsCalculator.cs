using Api.Dtos.Employee;
using Api.Services.PaycheckCalculators;
using Microsoft.Extensions.Options;

public interface ISalaryBasedBenefitsCalculator
{
    /// <summary>
    /// Calculates the salary based benefits costs for an employee per paycheck
    /// </summary>
    /// <param name="employee">Employee to calculate the costs for</param>
    /// <returns>The cost per paycheck</returns>
    decimal CalculateBenefitsCostsPerPaycheck(GetEmployeeDto employee);
}

public class SalaryBasedBenefitsCalculator : ISalaryBasedBenefitsCalculator
{
    private readonly IOptions<BenefitsCostSettings> _benefitsCostSettings;
    private readonly IPaycheckConversionHelper _paycheckConversionHelper;

    public SalaryBasedBenefitsCalculator(IPaycheckConversionHelper paycheckConversionHelper, IOptions<BenefitsCostSettings> benefitsCostSettings)
    {
        _paycheckConversionHelper = paycheckConversionHelper;
        _benefitsCostSettings = benefitsCostSettings;
    }
    
    public decimal CalculateBenefitsCostsPerPaycheck(GetEmployeeDto employee)
    {
        var cost = 0m;
        if (employee.Salary > _benefitsCostSettings.Value.SalaryBasedCosts?.SalaryThreshold)
        {
            var yearlyCost = (_benefitsCostSettings.Value.SalaryBasedCosts?.AdditionalYearlyCostPercent ?? 0) * employee.Salary;
            cost = _paycheckConversionHelper.ConvertYearlyAmountToPaycheckAmount(yearlyCost);
        }

        return cost;
    }
}