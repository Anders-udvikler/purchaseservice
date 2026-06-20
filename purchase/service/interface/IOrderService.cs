using Purchase.Models;

namespace service;

public interface IOrderService
{
    Task<Order> AddOrder(Order order);
    Task<List<Order>> GetAllOrders();
    Task<Order?> GetOrderById(string id);
    Task DeleteOrder(string id);
    Task UpdateOrder(Order order);
}