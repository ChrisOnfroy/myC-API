namespace MyApp.Application.DTOs;

public sealed record ProductResponseDto(
    Guid Id,
    string Sku,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid CategoryId,
    string? CategoryName,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record CreateProductRequestDto(
    string Sku,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid CategoryId);

public sealed record UpdateProductRequestDto(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid CategoryId);
