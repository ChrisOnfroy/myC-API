namespace MyApp.Application.Validations;

public sealed class ApplicationValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ApplicationValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("Ocurrieron uno o más errores de validación en la solicitud.")
    {
        Errors = errors;
    }

    public ApplicationValidationException(string propertyName, string errorMessage)
        : base(errorMessage)
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, [errorMessage] }
        };
    }

    public ApplicationValidationException(string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>
        {
            { "General", [message] }
        };
    }
}
