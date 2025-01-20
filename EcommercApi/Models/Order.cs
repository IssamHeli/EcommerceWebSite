namespace EcommercApi.Models;


public class Order
{
    public int Id { get; set; } // Primary Key
    public string CustomerName { get; set; } = string.Empty; // Customer's name
    public string CustomerEmail { get; set; } = string.Empty; // Email for contact
    public string CustomerAddress { get; set; } = string.Empty; // Delivery address
    public string CustomerPhone { get; set; } = string.Empty; // Phone number for contact
    public DateTime OrderDate { get; set; } = DateTime.UtcNow; // Order placement date
    public string Status { get; set; } = "Pending"; // Order status ("Pending", "Delivered", "Confirmed")

    public int ProductId { get; set; } // Foreign Key to Product
    public int Quantity { get; set; } // Total order amount
}
