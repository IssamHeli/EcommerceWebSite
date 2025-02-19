using EcommercApi.Data;
using EcommercApi.Models;
using EcommercApi.Services;
using EcommercApi.DtoClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _orderService.GetOrdersAsync();
        if (orders == null || !orders.Any())
        {
            return NotFound();
        }

        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> AddOrder(OrderDto orderDto)
    {
        var order = await _orderService.AddOrderAsync(orderDto);
        if (order == null)
        {
            return NotFound(new { message = "Order not Send , please try again ." });
        }

        return Ok(new { message = "Order created successfully.", order });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var result = await _orderService.DeleteOrderAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Order not found." });
        }

        return Ok(new { message = "Order deleted successfully." });
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
    {
        var validStatuses = new[] { "Delivered", "Confirmed", "Return" };
        if (!validStatuses.Contains(status))
        {
            return BadRequest(new { message = "Invalid status. Allowed values are 'Delivered', 'Confirmed', or 'Return'." });
        }

        var order = await _orderService.UpdateOrderStatusAsync(id, status);
        if (order == null)
        {
            return NotFound(new { message = "Order not found." });
        }

        return Ok(new { message = "Order status updated successfully.", order });
    }

    [HttpGet("getanalyse")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOrderAnalysis()
    {
        var analysis = await _orderService.GetOrderAnalysisAsync();
        return Ok(analysis);
    }
}