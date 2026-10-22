using MyApp.Domain.Abstraccions;

namespace MyApp.Domain.Entities;

/// <summary>
/// Entidad que representa un Producto en el catálogo.
/// Usa tipos simples como decimal para el precio y string para el SKU.
/// </summary>
public class Product : BaseEntity
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Método básico para actualizar los datos del producto.
    /// </summary>
    public void Update(string name, string description, decimal price, int stock, Guid categoryId)
    {
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Desactiva el producto en catálogo.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
