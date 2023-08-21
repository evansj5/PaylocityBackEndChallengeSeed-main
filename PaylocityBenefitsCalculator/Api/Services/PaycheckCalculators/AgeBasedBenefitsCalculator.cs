using Api.Dtos.Employee;
using Microsoft.Extensions.Options;

namespace Api.Services.PaycheckCalculators
{
    public interface IAgeBasedBenefitsCalculator
    {
        /// <summary>
        /// Calculates the age based benefits costs for an employee per paycheck
        /// </summary>
        /// <param name="employee">Employee to calculate</param>
        /// <returns>Age based benefits costs per paycheck</returns>
        decimal CalculateBenefitsCostsPerPaycheck(GetEmployeeDto employee);
    }

    public class AgeBasedBenefitsCalculator : IAgeBasedBenefitsCalculator
    {
        private readonly IPaycheckConversionHelper _paycheckConversionHelper;
        private readonly IOptions<BenefitsCostSettings> _benefitsCostSettings;

        public AgeBasedBenefitsCalculator(IPaycheckConversionHelper paycheckConversionHelper, IOptions<BenefitsCostSettings> benefitsCostSettings)
        {
            _paycheckConversionHelper = paycheckConversionHelper;
            _benefitsCostSettings = benefitsCostSettings;
        }
        
        public decimal CalculateBenefitsCostsPerPaycheck(GetEmployeeDto employee)
        {
            var age = employee.DateOfBirth.CalculateAge();
            var cost = 0m;
            if (age > _benefitsCostSettings.Value.AgeBasedCosts?.AgeThreshold)
            {
                cost = _benefitsCostSettings.Value.AgeBasedCosts?.AdditionalMonthlyCost ?? 0;
            }
            return _paycheckConversionHelper.ConvertMonthlyAmountToPaycheckAmount(cost);
        }
    }
}