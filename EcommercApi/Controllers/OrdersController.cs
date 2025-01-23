using Microsoft.AspNetCore.Http;
using EcommercApi.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;  // For BCrypt password hashing and verification
using System.IdentityModel.Tokens.Jwt;  // For working with JWT
using System.Security.Claims;  // For Claims
using Microsoft.IdentityModel.Tokens;  // For SecurityKey, SigningCredentials, and other JWT-related classes
using System.Text;  // For Encoding
using Microsoft.EntityFrameworkCore;
using EcommercApi.Data;
using Microsoft.AspNetCore.Authorization;


[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly EcommerceDbContext _context;

    public OrdersController(EcommerceDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public  async Task<IActionResult> GetOrders ()
    {
        var orderss = await _context.Orders.ToListAsync();

        var products = await _context.Products.ToListAsync();

        List<Orderswithproductinfo> orderswithproductinfo = new List<Orderswithproductinfo>();

        foreach (var order in orderss)
        {
            var productinfo = products.FirstOrDefault(p => p.Id == order.ProductId);
            orderswithproductinfo.Add(new Orderswithproductinfo
            {
                order = new OrderDto
                {
                    Id = order.Id,
                    CustomerName = order.CustomerName,
                    CustomerEmail = order.CustomerEmail,
                    CustomerAddress = order.CustomerAddress,
                    CustomerPhone = order.CustomerPhone,
                    ProductId = order.ProductId,
                    Quantity = order.Quantity,
                    OrderDate = order.OrderDate,
                    Status = order.Status
                },
                product = productinfo
            });
        }
        if(orderss == null)
        {
            return NotFound();
        }

        if(products == null)
        {
            return NotFound();
        }   

        if(orderswithproductinfo != null)
        {
            return Ok(orderswithproductinfo);
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> AddOrder(OrderDto orderDto)
    {
        // Validate the product ID
        var product = await _context.Products.FindAsync(orderDto.ProductId);
        if (product == null)
        {
            return NotFound(new { message = "Product not found." });
        }

        // Create the order
        var order = new Order
        {
            CustomerName = orderDto.CustomerName,
            CustomerEmail = orderDto.CustomerEmail,
            CustomerAddress = orderDto.CustomerAddress,
            CustomerPhone = orderDto.CustomerPhone,
            ProductId = orderDto.ProductId,
            Quantity = orderDto.Quantity,
            OrderDate = DateTime.UtcNow,
            Status = "Pending"
        };

        // Save to the database
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Order created successfully.", order });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        // Find the order by ID
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound(new { message = "Order not found." });
        }

        // Remove the order from the database
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Order deleted successfully." });
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
    {
        // Validate the status value
        var validStatuses = new[] { "Delivered", "Confirmed" , "Return"};
        if (!validStatuses.Contains(status))
        {
            return BadRequest(new { message = "Invalid status. Allowed values are 'Delivered' or 'Confirmed'.'Return" });
        }

        // Find the order by ID
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound(new { message = "Order not found." });
        }

        // Update the status
        order.Status = status;

        // Save changes to the database
        await _context.SaveChangesAsync();

        return Ok(new { message = "Order status updated successfully.", order });
    }

    [HttpGet("getanalyse")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> getanalyse(int id)
    {
        var Confirmed =  new List<Order>();
        var Delivered = new List<Order>();
        var Return = new List<Order>();
        var orders = await _context.Orders.ToListAsync();
        var totalorders = orders.Count();

        foreach (var order in orders)
        {
            if(order.Status == "Confirmed")
            {
                Confirmed.Add(order);
            }
            else if(order.Status == "Delivered")
            {
                Delivered.Add(order);
            }
            else if(order.Status == "Return")
            {
                Return.Add(order);
            }
        }

        decimal ConfirmedPercentage = (Confirmed.Count()  * 100) / totalorders;
        decimal DeliveredPercentage = (Delivered.Count()  * 100) / totalorders;
        decimal ReturnPercentage = (Return.Count()  * 100) / totalorders;

        var analyses = new ordersanalyses
        {
            totalorders = totalorders,
            ConfirmedPercentage = ConfirmedPercentage,
            DeliveredPercentage = DeliveredPercentage,
            ReturnPercentage = ReturnPercentage
        };
        return Ok(analyses);

    }


}

public class Orderswithproductinfo
{

    public OrderDto? order { get; set; } 
    public Product? product { get; set; }
}

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
public class ordersanalyses
{
    public int totalorders { get; set; }
    public decimal ConfirmedPercentage { get; set; }
    public decimal DeliveredPercentage { get; set; }
    public decimal ReturnPercentage { get; set; }
}

