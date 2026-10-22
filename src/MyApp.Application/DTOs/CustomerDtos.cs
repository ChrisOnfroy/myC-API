namespace MyApp.Application.DTOs;

public sealed record CustomerResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record CreateCustomerRequestDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber);

public sealed record UpdateCustomerRequestDto(
    string FirstName,
    string LastName,
    string PhoneNumber);
