using FluentValidation;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces;
using MyApp.Application.Validations;
using MyApp.Domain.Entities;
using MyApp.Domain.Errors;

namespace MyApp.Application.Services;

/// <summary>
/// Servicio de aplicación que implementa los casos de uso para clientes.
/// </summary>
public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCustomerRequestDto> _createValidator;
    private readonly IValidator<UpdateCustomerRequestDto> _updateValidator;

    public CustomerService(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateCustomerRequestDto> createValidator,
        IValidator<UpdateCustomerRequestDto> updateValidator)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<CustomerResponseDto>> GetAllAsync(bool onlyActive = false, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Customer> customers = await _customerRepository.GetAllAsync(onlyActive, cancellationToken);
        return customers.Select(MapToResponseDto).ToList();
    }

    public async Task<CustomerResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Customer? customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            throw new CustomerNotFoundException(id);
        }

        return MapToResponseDto(customer);
    }

    public async Task<CustomerResponseDto> CreateAsync(CreateCustomerRequestDto request, CancellationToken cancellationToken = default)
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

        // 2. Verificar que no exista un cliente con el mismo correo
        bool emailExists = await _customerRepository.ExistsByEmailAsync(request.Email, null, cancellationToken);
        if (emailExists)
        {
            throw new BadRequestException($"Ya existe un cliente registrado con el correo '{request.Email}'.");
        }

        // 3. Crear la entidad de forma simple y directa
        var customer = new Customer
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PhoneNumber = request.PhoneNumber.Trim()
        };

        // 4. Guardar en la base de datos
        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Retornar el DTO de respuesta
        return MapToResponseDto(customer);
    }

    public async Task<CustomerResponseDto> UpdateAsync(Guid id, UpdateCustomerRequestDto request, CancellationToken cancellationToken = default)
    {
        // 1. Validar los datos
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new ApplicationValidationException(errors);
        }

        // 2. Buscar si el cliente existe
        Customer? customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            throw new CustomerNotFoundException(id);
        }

        // 3. Actualizar datos con el método simple de la entidad
        customer.Update(request.FirstName.Trim(), request.LastName.Trim(), request.PhoneNumber.Trim());

        _customerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponseDto(customer);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Customer? customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            throw new CustomerNotFoundException(id);
        }

        customer.Deactivate();
        _customerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static CustomerResponseDto MapToResponseDto(Customer customer)
    {
        return new CustomerResponseDto(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.PhoneNumber,
            customer.IsActive,
            customer.CreatedAt,
            customer.UpdatedAt);
    }
}
