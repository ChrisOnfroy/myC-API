using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Interfaces;
using MyApp.Infraestructure.Persistence;
using MyApp.Infraestructure.Persistence.Repositories;
using MyApp.Infraestructure.Services;

namespace MyApp.Infraestructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        bool useInMemory = bool.TryParse(configuration["UseInMemoryDatabase"], out bool inMemory) && inMemory;
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (useInMemory || string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseInMemoryDatabase("MyAppDb");
            }
            else
            {
                options.UseSqlite(connectionString);
            }
        });

        // Repositorios y Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        // Servicios de Infraestructura
        services.AddSingleton<IDateTimeService, DateTimeService>();

        return services;
    }
}
