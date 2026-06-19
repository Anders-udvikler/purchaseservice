using HotChocolate;
using Furnitures;
using Purchase.Models;
using service;
using models;
using Stripe;
using service.Grapql;

namespace query
{
    public class Query
    {

/// <summary>
/// 
/// </summary>
/// <param name="repo"></param>
/// <param name="id"></param>
/// <returns></returns>
        public async Task<Order> GetOrderById(
            [Service] OrderService repo,string id
        )
        {
            var order = await repo.GetOrderById(id);
            return order;
        }

        
    }
}