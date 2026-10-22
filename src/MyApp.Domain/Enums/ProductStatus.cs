namespace MyApp.Domain.Enums;

/// <summary>
/// Estados posibles de un producto en el catálogo.
/// </summary>
public enum ProductStatus
{
    /// <summary>
    /// Producto activo y disponible para la venta.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Producto inactivo o pausado.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Producto descontinuado definitivamente.
    /// </summary>
    Discontinued = 3
}
