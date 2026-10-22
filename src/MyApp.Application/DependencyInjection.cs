using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces;
using MyApp.Application.Services;
using MyApp.Application.Validations;

namespace MyApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registro explícito de Validadores FluentValidation
        services.AddScoped<IValidator<CreateCustomerRequestDto>, CreateCustomerRequestValidator>();
        services.AddScoped<IValidator<UpdateCustomerRequestDto>, UpdateCustomerRequestValidator>();
        services.AddScoped<IValidator<CreateCategoryRequestDto>, CreateCategoryRequestValidator>();
        services.AddScoped<IValidator<UpdateCategoryRequestDto>, UpdateCategoryRequestValidator>();
        services.AddScoped<IValidator<CreateProductRequestDto>, CreateProductRequestValidator>();
        services.AddScoped<IValidator<UpdateProductRequestDto>, UpdateProductRequestValidator>();

        // Registro de Servicios de Aplicación
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
