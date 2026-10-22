using FluentValidation;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces;
using MyApp.Application.Validations;
using MyApp.Domain.Entities;
using MyApp.Domain.Errors;

namespace MyApp.Application.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCategoryRequestDto> _createValidator;
    private readonly IValidator<UpdateCategoryRequestDto> _updateValidator;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateCategoryRequestDto> createValidator,
        IValidator<UpdateCategoryRequestDto> updateValidator)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync(bool onlyActive = false, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Category> categories = await _categoryRepository.GetAllAsync(onlyActive, cancellationToken);
        return categories.Select(MapToResponseDto).ToList();
    }

    public async Task<CategoryResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Category? category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            throw new CategoryNotFoundException(id);
        }

        return MapToResponseDto(category);
    }

    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryRequestDto request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new ApplicationValidationException(errors);
        }

        bool nameExists = await _categoryRepository.ExistsByNameAsync(request.Name, null, cancellationToken);
        if (nameExists)
        {
            throw new BadRequestException($"Ya existe una categoría con el nombre '{request.Name}'.");
        }

        var category = new Category
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim() ?? string.Empty
        };

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponseDto(category);
    }

    public async Task<CategoryResponseDto> UpdateAsync(Guid id, UpdateCategoryRequestDto request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new ApplicationValidationException(errors);
        }

        Category? category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            throw new CategoryNotFoundException(id);
        }

        bool nameExists = await _categoryRepository.ExistsByNameAsync(request.Name, id, cancellationToken);
        if (nameExists)
        {
            throw new BadRequestException($"Ya existe otra categoría con el nombre '{request.Name}'.");
        }

        category.Update(request.Name.Trim(), request.Description?.Trim() ?? string.Empty);

        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponseDto(category);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Category? category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            throw new CategoryNotFoundException(id);
        }

        category.Deactivate();
        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static CategoryResponseDto MapToResponseDto(Category category)
    {
        return new CategoryResponseDto(
            category.Id,
            category.Name,
            category.Description,
            category.IsActive,
            category.CreatedAt,
            category.UpdatedAt);
    }
}
