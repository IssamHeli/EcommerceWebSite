using EcommercApi.Data;
using EcommercApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EcommercApi.DtoClasses ;

public class ProductService : IProductService
{
    private readonly EcommerceDbContext _context;

    public ProductService(EcommerceDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult> GetProductsAsync(int page, int pageSize, string search, string category, decimal? minPrice, decimal? maxPrice)
    {
        IQueryable<Product> query = _context.Products;

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category == category);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        var totalCount = await query.CountAsync();
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new PagedResult
        {
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            CurrentPage = page,
            PageSize = pageSize,
            Items = products
        };

        return result;
    }

    public async Task<Product> GetProductAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<List<Product>> GetRelatedProductsAsync(string category, int productId)
    {
        return await _context.Products
            .Where(p => p.Category == category && p.Id != productId)
            .Take(6)
            .ToListAsync();
    }

    public async Task<bool> UpdateProductAsync(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Products.Any(p => p.Id == product.Id))
            {
                return false;
            }
            throw;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return false;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}