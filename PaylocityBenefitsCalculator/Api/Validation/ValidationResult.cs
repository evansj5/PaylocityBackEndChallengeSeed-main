public class ValidationResult
{
    public bool IsValid { get; }
    public string ErrorMessage { get; }

    public ValidationResult(bool isValid, string errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success() => new ValidationResult(true);
    public static ValidationResult Failure(string errorMessage) => new ValidationResult(false, errorMessage);
}
