public class BenefitsCostSettings
{
    public int PaychecksPerYear { get; set; }
    public decimal EmployeeCostPerMonth { get; set; }
    public decimal DependentCostPerMonth { get; set; }
    public SalaryBasedCostsSettings SalaryBasedCosts { get; set; }
    public AgeBasedCostsSettings AgeBasedCosts { get; set; }
}

public class SalaryBasedCostsSettings
{
    public decimal SalaryThreshold { get; set; }
    public decimal AdditionalYearlyCostPercent { get; set; }
}

public class AgeBasedCostsSettings
{
    public int AgeThreshold { get; set; }
    public decimal AdditionalMonthlyCost { get; set; }
    public int DependentAgeThreshold { get; set; }
    public decimal DependentAdditionalMonthlyCost { get; set; }
}