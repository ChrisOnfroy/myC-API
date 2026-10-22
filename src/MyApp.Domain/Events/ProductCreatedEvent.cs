namespace MyApp.Domain.Events;

/// <summary>
/// Evento de dominio emitido cuando se crea un nuevo producto en catálogo.
/// </summary>
/// <param name="ProductId">Identificador del producto.</param>
/// <param name="Sku">Código SKU asignado.</param>
public sealed record ProductCreatedEvent(Guid ProductId, string Sku) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
