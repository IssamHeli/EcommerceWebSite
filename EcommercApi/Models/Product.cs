
namespace EcommercApi.Models;



public class Product
{
    public int Id { get; set; } // Primary Key
    public string Name { get; set; } = string.Empty; // Product name
    public string Description { get; set; } = string.Empty; // Product description
    public string Category { get; set; } = string.Empty; // Product description
    public decimal Price { get; set; } // Price of the product
    public int Stock { get; set; } // Quantity available
    public string ImageUrl { get; set; } = string.Empty; // URL to the product image
}
