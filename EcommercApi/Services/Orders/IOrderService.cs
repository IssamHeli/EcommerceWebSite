using EcommercApi.Models;
using EcommercApi.DtoClasses;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IOrderService
{
    Task<List<Orderswithproductinfo>> GetOrdersAsync();
    Task<Order> AddOrderAsync(OrderDto orderDto);
    Task<bool> DeleteOrderAsync(int id);
    Task<Order> UpdateOrderStatusAsync(int id, string status);
    Task<Ordersanalyses> GetOrderAnalysisAsync();
}