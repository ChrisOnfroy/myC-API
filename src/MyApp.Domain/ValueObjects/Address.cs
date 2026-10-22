namespace MyApp.Domain.ValueObjects;

/// <summary>
/// Ejemplo didáctico de Objeto de Valor (Value Object).
/// En C# moderno, los Value Objects se representan fácilmente con records, 
/// ya que comparan por valor y no por identidad sin necesidad de clases complejas.
/// </summary>
/// <param name="Street">Calle y número.</param>
/// <param name="City">Ciudad.</param>
/// <param name="Country">País.</param>
public record Address(string Street, string City, string Country);
