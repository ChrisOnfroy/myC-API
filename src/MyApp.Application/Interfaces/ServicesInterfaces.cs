using MyApp.Application.DTOs;

namespace MyApp.Application.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerResponseDto>> GetAllAsync(bool onlyActive = false, CancellationToken cancellationToken = default);
    Task<CustomerResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CustomerResponseDto> CreateAsync(CreateCustomerRequestDto request, CancellationToken cancellationToken = default);
    Task<CustomerResponseDto> UpdateAsync(Guid id, UpdateCustomerRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync(bool onlyActive = false, CancellationToken cancellationToken = default);
    Task<CategoryResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CategoryResponseDto> CreateAsync(CreateCategoryRequestDto request, CancellationToken cancellationToken = default);
    Task<CategoryResponseDto> UpdateAsync(Guid id, UpdateCategoryRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IProductService
{
    Task<IReadOnlyList<ProductResponseDto>> GetAllAsync(Guid? categoryId = null, bool onlyActive = false, CancellationToken cancellationToken = default);
    Task<ProductResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductResponseDto> CreateAsync(CreateProductRequestDto request, CancellationToken cancellationToken = default);
    Task<ProductResponseDto> UpdateAsync(Guid id, UpdateProductRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IDateTimeService
{
    DateTime UtcNow { get; }
}
