using EcommercApi.Data;
using EcommercApi.Models;
using EcommercApi.DtoClasses;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class OrderService : IOrderService
{
    private readonly EcommerceDbContext _context;

    public OrderService(EcommerceDbContext context)
    {
        _context = context;
    }

    public async Task<List<Orderswithproductinfo>> GetOrdersAsync()
    {
        var orders = await _context.Orders.ToListAsync();
        var products = await _context.Products.ToListAsync();

        var orderswithproductinfo = new List<Orderswithproductinfo>();

        foreach (var order in orders)
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

        return orderswithproductinfo;
    }

    public async Task<Order> AddOrderAsync(OrderDto orderDto)
    {
        var product = await _context.Products.FindAsync(orderDto.ProductId);
        if (product == null)
        {
            return null;
        }

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

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return false;
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<Order> UpdateOrderStatusAsync(int id, string status)
    {
        var validStatuses = new[] { "Delivered", "Confirmed", "Return" };
        if (!validStatuses.Contains(status))
        {
            return null;
        }

        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return null;
        }

        order.Status = status;
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<Ordersanalyses> GetOrderAnalysisAsync()
    {
        var orders = await _context.Orders.ToListAsync();
        var totalorders = orders.Count;

        var confirmed = orders.Count(o => o.Status == "Confirmed");
        var delivered = orders.Count(o => o.Status == "Delivered");
        var returned = orders.Count(o => o.Status == "Return");
        var pending = orders.Count(o => o.Status == "Pending");

        var analyses = new Ordersanalyses
        {
            Totalorders = totalorders,
            ConfirmedPercentage = (double)confirmed / totalorders * 100,
            DeliveredPercentage = (double)delivered / totalorders * 100,
            ReturnPercentage = (double)returned / totalorders * 100,
            PendingPercentage = (double)pending / totalorders * 100
        };

        return analyses;
    }
}