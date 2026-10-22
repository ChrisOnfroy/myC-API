using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyApp.Domain.Entities;

namespace MyApp.Infraestructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        ILogger logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DatabaseSeeder));

        try
        {
            await context.Database.EnsureCreatedAsync();

            if (!await context.Categories.AnyAsync())
            {
                logger.LogInformation("Sembrando categorías...");

                var categoryTech = new Category
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Tecnología",
                    Description = "Dispositivos electrónicos, componentes de computación y accesorios.",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var categoryBooks = new Category
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Libros y Educación",
                    Description = "Libros técnicos, literatura de desarrollo y guías de arquitectura.",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var categoryHome = new Category
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Hogar y Oficina",
                    Description = "Mobiliario ergonómico y artículos para estaciones de trabajo.",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Categories.AddRangeAsync(categoryTech, categoryBooks, categoryHome);
                await context.SaveChangesAsync();

                logger.LogInformation("Sembrando productos...");

                var prodLaptop = new Product
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Sku = "TECH-LAPTOP-001",
                    Name = "Laptop Developer Pro 16",
                    Description = "Laptop de alto rendimiento equipada con 32GB RAM y procesador de 16 núcleos.",
                    Price = 1499.99m,
                    Stock = 25,
                    CategoryId = categoryTech.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var prodBook = new Product
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Sku = "BOOK-CLEAN-ARCH",
                    Name = "Clean Architecture: Guía de Diseño y Buenas Prácticas",
                    Description = "Libro de referencia sobre arquitectura limpia, patrones y principios SOLID.",
                    Price = 39.50m,
                    Stock = 100,
                    CategoryId = categoryBooks.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var prodChair = new Product
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Sku = "HOME-CHAIR-ERGO",
                    Name = "Silla Ergonómica DeskMaster",
                    Description = "Silla con soporte lumbar ajustable y reposabrazos 4D.",
                    Price = 289.00m,
                    Stock = 15,
                    CategoryId = categoryHome.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Products.AddRangeAsync(prodLaptop, prodBook, prodChair);
                await context.SaveChangesAsync();
            }

            if (!await context.Customers.AnyAsync())
            {
                logger.LogInformation("Sembrando clientes...");

                var customer1 = new Customer
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    FirstName = "Ian",
                    LastName = "Gómez",
                    Email = "ian.gomez@riwi.io",
                    PhoneNumber = "+57 300 1234567",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var customer2 = new Customer
                {
                    Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    FirstName = "Laura",
                    LastName = "Martínez",
                    Email = "laura.martinez@riwi.io",
                    PhoneNumber = "+57 310 9876543",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Customers.AddRangeAsync(customer1, customer2);
                await context.SaveChangesAsync();
            }

            logger.LogInformation("Base de datos inicializada con éxito.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error durante la siembra de base de datos.");
            throw;
        }
    }
}
