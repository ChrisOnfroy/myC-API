using FluentValidation;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces;
using MyApp.Application.Validations;
using MyApp.Domain.Entities;
using MyApp.Domain.Errors;

namespace MyApp.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateProductRequestDto> _createValidator;
    private readonly IValidator<UpdateProductRequestDto> _updateValidator;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateProductRequestDto> createValidator,
        IValidator<UpdateProductRequestDto> updateValidator)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<ProductResponseDto>> GetAllAsync(
        Guid? categoryId = null,
        bool onlyActive = false,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Product> products = await _productRepository.GetAllAsync(categoryId, onlyActive, cancellationToken);
        return products.Select(MapToResponseDto).ToList();
    }

    public async Task<ProductResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Product? product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            throw new ProductNotFoundException(id);
        }

        return MapToResponseDto(product);
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductRequestDto request, CancellationToken cancellationToken = default)
    {
        // 1. Validar los datos de entrada
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new ApplicationValidationException(errors);
        }

        // 2. Verificar que no exista un producto con el mismo SKU
        bool skuExists = await _productRepository.ExistsBySkuAsync(request.Sku, null, cancellationToken);
        if (skuExists)
        {
            throw new BadRequestException($"Ya existe un producto con el SKU '{request.Sku}'.");
        }

        // 3. Verificar que la categoría exista
        Category? category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
        {
            throw new CategoryNotFoundException(request.CategoryId);
        }

        // 4. Crear el producto usando propiedades simples
        var product = new Product
        {
            Sku = request.Sku.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId
        };

        // 5. Guardar en base de datos
        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductResponseDto(
            product.Id,
            product.Sku,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CategoryId,
            category.Name,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt);
    }

    public async Task<ProductResponseDto> UpdateAsync(Guid id, UpdateProductRequestDto request, CancellationToken cancellationToken = default)
    {
        // 1. Validar datos
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new ApplicationValidationException(errors);
        }

        // 2. Verificar que el producto exista
        Product? product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            throw new ProductNotFoundException(id);
        }

        // 3. Verificar que la nueva categoría exista
        Category? category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
        {
            throw new CategoryNotFoundException(request.CategoryId);
        }

        // 4. Actualizar mediante el método simple de la entidad
        product.Update(request.Name.Trim(), request.Description?.Trim() ?? string.Empty, request.Price, request.Stock, request.CategoryId);

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductResponseDto(
            product.Id,
            product.Sku,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CategoryId,
            category.Name,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Product? product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            throw new ProductNotFoundException(id);
        }

        product.Deactivate();
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static ProductResponseDto MapToResponseDto(Product product)
    {
        return new ProductResponseDto(
            product.Id,
            product.Sku,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CategoryId,
            product.Category?.Name,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt);
    }
}
