using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;

namespace MyApp.Infraestructure.Persistence.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CustomerRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        string normalized = email.Trim().ToLowerInvariant();
        return await _dbContext.Customers.FirstOrDefaultAsync(c => c.Email.ToLower() == normalized, cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(bool onlyActive = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Customer> query = _dbContext.Customers.AsNoTracking();
        if (onlyActive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query.OrderByDescending(c => c.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customer);
        await _dbContext.Customers.AddAsync(customer, cancellationToken);
    }

    public void Update(Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer);
        _dbContext.Customers.Update(customer);
    }

    public void Remove(Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer);
        _dbContext.Customers.Remove(customer);
    }

    public async Task<bool> ExistsByEmailAsync(string email, Guid? excludeCustomerId = null, CancellationToken cancellationToken = default)
    {
        string normalized = email.Trim().ToLowerInvariant();
        IQueryable<Customer> query = _dbContext.Customers.AsNoTracking();
        if (excludeCustomerId.HasValue)
        {
            query = query.Where(c => c.Id != excludeCustomerId.Value);
        }

        return await query.AnyAsync(c => c.Email.ToLower() == normalized, cancellationToken);
    }
}

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        string normalized = name.Trim().ToLowerInvariant();
        return await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == normalized, cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(bool onlyActive = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Category> query = _dbContext.Categories.AsNoTracking();
        if (onlyActive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query.OrderBy(c => c.Name).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(category);
        await _dbContext.Categories.AddAsync(category, cancellationToken);
    }

    public void Update(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        _dbContext.Categories.Update(category);
    }

    public void Remove(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        _dbContext.Categories.Remove(category);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeCategoryId = null, CancellationToken cancellationToken = default)
    {
        string normalized = name.Trim().ToLowerInvariant();
        IQueryable<Category> query = _dbContext.Categories.AsNoTracking();
        if (excludeCategoryId.HasValue)
        {
            query = query.Where(c => c.Id != excludeCategoryId.Value);
        }

        return await query.AnyAsync(c => c.Name.ToLower() == normalized, cancellationToken);
    }
}

public sealed class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        string normalized = sku.Trim().ToUpperInvariant();
        return await _dbContext.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Sku.ToUpper() == normalized, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(Guid? categoryId = null, bool onlyActive = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = _dbContext.Products
            .Include(p => p.Category)
            .AsNoTracking();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (onlyActive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product);
        await _dbContext.Products.AddAsync(product, cancellationToken);
    }

    public void Update(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        _dbContext.Products.Update(product);
    }

    public void Remove(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        _dbContext.Products.Remove(product);
    }

    public async Task<bool> ExistsBySkuAsync(string sku, Guid? excludeProductId = null, CancellationToken cancellationToken = default)
    {
        string normalized = sku.Trim().ToUpperInvariant();
        IQueryable<Product> query = _dbContext.Products.AsNoTracking();
        if (excludeProductId.HasValue)
        {
            query = query.Where(p => p.Id != excludeProductId.Value);
        }

        return await query.AnyAsync(p => p.Sku.ToUpper() == normalized, cancellationToken);
    }
}
