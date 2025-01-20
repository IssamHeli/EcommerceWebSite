using Microsoft.AspNetCore.Http;
using EcommercApi.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;  // For BCrypt password hashing and verification
using System.IdentityModel.Tokens.Jwt;  // For working with JWT
using System.Security.Claims;  // For Claims
using Microsoft.IdentityModel.Tokens;  // For SecurityKey, SigningCredentials, and other JWT-related classes
using System.Text;  // For Encoding
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using EcommercApi.Data;


[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly EcommerceDbContext _context;

    public ProductsController(EcommerceDbContext context)
    {
        _context = context;
    }

    // GET /api/products?page=1&pageSize=10&search=phone&category=Electronics&minPrice=100&maxPrice=1000
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        int page = 1, 
        int pageSize = 10, 
        string search = "", 
        string category = "", 
        decimal? minPrice = null, 
        decimal? maxPrice = null)
    {
        IQueryable<Product> query = _context.Products;

        // Search by product name
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        // Filter by category
        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category == category);
        }

        // Filter by price range
        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        // Pagination
        var totalCount = await query.CountAsync();
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new
        {
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            CurrentPage = page,
            PageSize = pageSize,
            Products = products
        };

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddProduct([FromBody] Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
    {
        if (id != product.Id)
        {
            return BadRequest();
        }

        _context.Entry(product).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Products.Any(p => p.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }

/*
    [HttpGet("best-sellers")]
    public IActionResult GetBestSellers(int page = 1)
    {
        var orders = _context.Orders.ToList();
        var products = _context.Products.ToList();
        var bestseller = new List<Product>();
        foreach (var product in products)
        {
            foreach (var order in orders)
            {
                if (order.ProductId == product.Id)
                {
                    bestseller.Add(product);
                    
                }
            }
        }
        return Ok(bestseller);
    }
[HttpGet("recommended")]
public IActionResult GetRecommendedProducts(int page = 1)
{
    const int pageSize = 6;
    var recommendedProducts = _context.Products
        .Where(p => p.Stock >= 400)
        .OrderByDescending(p => p.Price)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    return Ok(new { products = recommendedProducts, page, pageSize });
}
*/
}
