using Microsoft.AspNetCore.Razor.TagHelpers;
using MongoDB.Driver;
using Purchase.Models;

namespace service
{
    public class OrderService
    {
        private readonly IMongoCollection<Order> _OrderCollection;

        /// <summary>
        /// Adds a new report to the MongoDB collection. The method takes a Report object as a parameter and inserts it into the collection. If the insertion is successful, it returns the added Report object. If an error occurs during the insertion process, it logs the error message and rethrows the exception.
        /// </summary>
        /// <param name="Report"></param>
        /// <returns></returns>
        public async Task<Order> AddOrder(Order order)
        {
            try
            {
                if(order.OrderItems==null)
                {
                    throw new Exception();
                }
                if(order.UserGuid==null)
                {
                    throw new Exception();
                }
                await _OrderCollection.InsertOneAsync(order);
                return order;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding report: {ex.Message}");
                throw;
            }

        }

        /// <summary>
        /// Retrieves all reports from the MongoDB collection. The method returns a list of Report objects representing all the reports stored in the collection. If an error occurs during the retrieval process, it logs the error message and rethrows the exception.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Order>> GetAllReports()
        {
            try
            {
                return await _OrderCollection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding report: {ex.Message}");
                throw;
            }
        }

/// <summary>
/// Retrieves a report from the MongoDB collection based on the provided ID. The method takes a string parameter representing the ID of the report to be retrieved and returns the corresponding Report object if found. If an error occurs during the retrieval process, it logs the error message and rethrows the exception.
/// </summary>
/// <param name="id">The ID of the report to retrieve.</param>
/// <returns>The retrieved Report object, or null if not found.</returns>
        public async Task<Order> GetOrderById(string id)
        { 
            try
            {
                return await _OrderCollection.Find(r => r.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding report: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteOrder(string id)
        {
            try
            {
                await _OrderCollection.DeleteOneAsync(r => r.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding report: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateOrder(Order order)
        {
            try
            {
                if(order.OrderItems==null)
                {
                    throw new Exception();
                }
                if(order.UserGuid==null)
                {
                    throw new Exception();
                }
                await _OrderCollection.ReplaceOneAsync(x => x.Id == order.Id,order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding report: {ex.Message}");
                throw;
            }
        }   
    }
}