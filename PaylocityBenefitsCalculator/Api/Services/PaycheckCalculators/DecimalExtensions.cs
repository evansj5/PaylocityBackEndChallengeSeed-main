namespace Api.Services.PaycheckCalculators
{
    public static class DecimalExtensions
    {
        /// <summary>
        /// Floors a decimal amount of money to the nearest cent. 1.5099999 becomes 1.50, kind of an arbitrary choice
        /// I would confirm with the product owners to see how this should be calculated in a system.
        /// </summary>
        /// <param name="value">Decimal value to floor to the nearest cent</param>
        /// <returns>Decimal</returns>
        public static decimal FloorToNearestCent(this decimal value)
        {
            return Math.Floor(value * 100) / 100;
        }
    }
}