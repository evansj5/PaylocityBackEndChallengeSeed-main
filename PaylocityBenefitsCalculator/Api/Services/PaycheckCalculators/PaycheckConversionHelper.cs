using Microsoft.Extensions.Options;

namespace Api.Services.PaycheckCalculators
{
    public interface IPaycheckConversionHelper
    {
        /// <summary>
        /// Converts a "per month" value to a "per paycheck" value using the paychecks per year value in the settings.
        /// </summary>
        /// <param name="monthlyAmount">The monthly amount we're converting</param>
        /// <returns>The paycheck amount</returns>
        decimal ConvertMonthlyAmountToPaycheckAmount(decimal monthlyAmount);

        /// <summary>
        /// Converts a "per year" value to a "per paycheck" value using the paychecks per year value in the settings.
        /// </summary>
        /// <param name="yearlyAmount">The yearly amount we're converting</param>
        /// <returns>The paycheck amount</returns>
        decimal ConvertYearlyAmountToPaycheckAmount(decimal yearlyAmount);
    }

    public class PaycheckConversionHelper : IPaycheckConversionHelper
    {
        private readonly IOptions<BenefitsCostSettings> _benefitsCostSettings;

        public PaycheckConversionHelper(IOptions<BenefitsCostSettings> benefitsCostSettings)
        {
            _benefitsCostSettings = benefitsCostSettings;
        }

        public decimal ConvertMonthlyAmountToPaycheckAmount(decimal monthlyAmount)
        {
            return monthlyAmount * 12 / _benefitsCostSettings.Value.PaychecksPerYear;
        }

        public decimal ConvertYearlyAmountToPaycheckAmount(decimal yearlyAmount)
        {
            return yearlyAmount / _benefitsCostSettings.Value.PaychecksPerYear;
        }
    }
}