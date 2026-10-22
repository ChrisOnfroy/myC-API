using MyApp.Domain.Abstraccions;

namespace MyApp.Domain.Entities;

/// <summary>
/// Entidad que representa a un Cliente en el sistema.
/// Hereda de BaseEntity para tener Id, CreatedAt y UpdatedAt automáticamente.
/// </summary>
public class Customer : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Método básico para actualizar los datos del cliente.
    /// Actualiza también la fecha UpdatedAt automáticamente.
    /// </summary>
    public void Update(string firstName, string lastName, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Desactiva al cliente (borrado lógico).
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
