namespace MyApp.Domain.Errors;

/// <summary>
/// Excepción lanzada cuando no se encuentra un recurso solicitado (mapea a 404 Not Found).
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}

/// <summary>
/// Excepción lanzada cuando hay un error en la solicitud o regla de negocio (mapea a 400 Bad Request).
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}

public sealed class CustomerNotFoundException : NotFoundException
{
    public CustomerNotFoundException(Guid id) : base($"No se encontró el cliente con ID '{id}'.") { }
}

public sealed class ProductNotFoundException : NotFoundException
{
    public ProductNotFoundException(Guid id) : base($"No se encontró el producto con ID '{id}'.") { }
}

public sealed class CategoryNotFoundException : NotFoundException
{
    public CategoryNotFoundException(Guid id) : base($"No se encontró la categoría con ID '{id}'.") { }
}
