namespace MyApp.Domain.Events;

/// <summary>
/// Contrato base para eventos de dominio en el sistema.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Identificador único del evento de dominio.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Fecha y hora en formato UTC en la que ocurrió el evento de dominio.
    /// </summary>
    DateTime OccurredOn { get; }
}
