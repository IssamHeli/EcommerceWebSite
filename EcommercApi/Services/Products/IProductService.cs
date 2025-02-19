using EcommercApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using EcommercApi.DtoClasses ;

public interface IProductService
{
    Task<PagedResult> GetProductsAsync(int page, int pageSize, string search, string category, decimal? minPrice, decimal? maxPrice);
    Task<Product> GetProductAsync(int id);
    Task<Product> AddProductAsync(Product product);
    Task<List<Product>> GetRelatedProductsAsync(string category, int productId);
    Task<bool> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int id);
}