namespace MyApp.Domain.Abstraccions;

/// <summary>
/// Clase base común para todas las entidades.
/// Contiene las propiedades estándar que toda tabla/entidad debe tener: Id, CreatedAt y UpdatedAt.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único de la entidad (GUID).
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Fecha y hora en que se creó el registro (en UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha y hora de la última modificación (nulo si nunca se ha modificado).
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
