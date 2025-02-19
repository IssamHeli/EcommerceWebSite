namespace EcommercApi.DtoClasses;


public class OrderDto
{

    public int Id { get; set; } // Primary Key
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow; 
    public string Status { get; set; } = "Pending"; 
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}