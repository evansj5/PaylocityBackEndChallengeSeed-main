using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Microsoft.Extensions.Options;

namespace Api.Services.PaycheckCalculators
{
    public interface IPaycheckCalculator
    {
        /// <summary>
        /// Calculates the paycheck for an employee
        /// </summary>
        /// <param name="employee">Employee to calculate the paycheck of</param>
        /// <returns><see cref="GetPaycheckDto"/> for the employee</returns>
        GetPaycheckDto CalculatePaycheck(GetEmployeeDto employee);
    }

    public class PaycheckCalculator : IPaycheckCalculator
    {
        private readonly IOptions<BenefitsCostSettings> _benefitsCostSettings;
        private readonly IPaycheckConversionHelper _paycheckConversionHelper;
        private readonly IAgeBasedBenefitsCalculator _ageBasedBenefitsCalculator;
        private readonly IDependentsBenefitsCalculator _dependentsBenefitsCalculator;
        private readonly ISalaryBasedBenefitsCalculator _salaryBasedBenefitsCalculator;

        public PaycheckCalculator(
            IOptions<BenefitsCostSettings> benefitsCostSettings, 
            IPaycheckConversionHelper paycheckConversionHelper,
            IAgeBasedBenefitsCalculator ageBasedBenefitsCalculator,
            IDependentsBenefitsCalculator dependentsBenefitsCalculator,
            ISalaryBasedBenefitsCalculator salaryBasedBenefitsCalculator
            )
        {
            _benefitsCostSettings = benefitsCostSettings;
            _paycheckConversionHelper = paycheckConversionHelper;
            _ageBasedBenefitsCalculator = ageBasedBenefitsCalculator;
            _dependentsBenefitsCalculator = dependentsBenefitsCalculator;
            _salaryBasedBenefitsCalculator = salaryBasedBenefitsCalculator;
        }
        public GetPaycheckDto CalculatePaycheck(GetEmployeeDto employee) {
            /** 
                *Completely* unsure how fractional cents should be handled in these types of systems. I am going to just lop off the fractional cents.
                I assume in a real HR system there would be some logic that trues that up throughout the year.
            **/

            // all of these amounts need to get normalized to a paycheck amount
            var grossPay = _paycheckConversionHelper.ConvertYearlyAmountToPaycheckAmount(employee.Salary).FloorToNearestCent();

            var benefitsCosts = _paycheckConversionHelper.ConvertMonthlyAmountToPaycheckAmount(_benefitsCostSettings.Value.EmployeeCostPerMonth).FloorToNearestCent();

            var dependentCosts = _dependentsBenefitsCalculator.CalculateBenefitsCostsPerPaycheck(employee).FloorToNearestCent();

            decimal ageBasedCosts = _ageBasedBenefitsCalculator.CalculateBenefitsCostsPerPaycheck(employee).FloorToNearestCent();

            decimal salaryBasedCosts = _salaryBasedBenefitsCalculator.CalculateBenefitsCostsPerPaycheck(employee).FloorToNearestCent();

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
    }
}