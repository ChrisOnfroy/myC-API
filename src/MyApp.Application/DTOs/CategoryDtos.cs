namespace MyApp.Application.DTOs;

public sealed record CategoryResponseDto(
    Guid Id,
    string Name,
    string Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record CreateCategoryRequestDto(
    string Name,
    string Description);

public sealed record UpdateCategoryRequestDto(
    string Name,
    string Description);
