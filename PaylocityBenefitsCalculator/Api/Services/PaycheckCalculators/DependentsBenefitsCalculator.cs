using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Api.Services.PaycheckCalculators;
using Microsoft.Extensions.Options;

public interface IDependentsBenefitsCalculator
{
    /// <summary>
    /// Calculates the benefits costs for all dependents per paycheck
    /// </summary>
    /// <param name="employee">Employee whose dependents we want to calculate the costs for</param>
    /// <returns>Cost of a dependent per paycheck</returns>
    decimal CalculateBenefitsCostsPerPaycheck(GetEmployeeDto employee);
}

public class DependentsBenefitsCalculator : IDependentsBenefitsCalculator
{
    private readonly IPaycheckConversionHelper _paycheckConversionHelper;
    private readonly IOptions<BenefitsCostSettings> _benefitsCostSettings;

    public DependentsBenefitsCalculator(IPaycheckConversionHelper paycheckConversionHelper, IOptions<BenefitsCostSettings> benefitsCostSettings)
    {
        _paycheckConversionHelper = paycheckConversionHelper;
        _benefitsCostSettings = benefitsCostSettings;
    }
    
    public decimal CalculateBenefitsCostsPerPaycheck(GetEmployeeDto employee)
    {
        var costs = 0m;
        foreach (var dependent in employee.Dependents) {
            var age = dependent.DateOfBirth.CalculateAge();
            var cost = _benefitsCostSettings.Value.DependentCostPerMonth;
            if (age > _benefitsCostSettings.Value.AgeBasedCosts?.DependentAgeThreshold)
            {
                cost += _benefitsCostSettings.Value.AgeBasedCosts?.DependentAdditionalMonthlyCost ?? 0;
            }
            costs += _paycheckConversionHelper.ConvertMonthlyAmountToPaycheckAmount(cost);
        }

        return costs;
    }
}