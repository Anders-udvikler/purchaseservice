
using HotChocolate;
using Furnitures;
using service;
using Purchase.Models;
using service.Grapql;
using Stripe;
using models;

namespace mutation
{
    public class Mutation
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public Task<Order> AddOrder(
        [Service] OrderService repo, Order order)
        {
            try
            {
                return repo.AddOrder(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding report: {ex.Message}");
                return null;
            }
        }

/// <summary>
/// 
/// </summary>
/// <param name="repo"></param>
/// <param name="order"></param>
/// <returns></returns>
        public async Task DeleteOrder(
        [Service] OrderService repo, Order order)
        {
            try
            {
                 await repo.DeleteOrder(order.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding report: {ex.Message}");
            }
        }
}}