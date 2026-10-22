namespace MyApp.Domain.Events;

/// <summary>
/// Evento de dominio emitido cuando se crea un nuevo cliente.
/// </summary>
/// <param name="CustomerId">Identificador del cliente.</param>
/// <param name="Email">Correo del cliente.</param>
public sealed record CustomerCreatedEvent(Guid CustomerId, string Email) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
