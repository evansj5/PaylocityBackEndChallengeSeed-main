public class GetPaycheckDto
{
    public decimal GrossPay { get; set; }
    public decimal BaseBenefitsCosts { get; set; }
    public decimal DependentsBenefitsCosts { get; set; }
    public decimal SalaryBenefitsCosts { get; set; }
    public decimal AgeBasedBenefitsCosts { get; set; }
    public decimal NetPay { get; set; }
}