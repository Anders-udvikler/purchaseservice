using Furnitureservice;
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
        /// Retrieves all furniture items from the MongoDB collection. The method returns a list of Furniture objects representing all the furniture items stored in the collection. If an error occurs during the retrieval process, it logs the error message and rethrows the exception.
        /// </summary>
        /// <param name="repo">The furniture service repository.</param>
        /// <returns>A list of all furniture items.</returns>
        public async Task<List<Furniture>> GetAllFurniture(
        [Service] FurnitureService repo)
        {
        var allFurniture = await repo.GetAllFurnitures();
        return allFurniture;
        }

/// <summary>
/// Retrieves a furniture item from the MongoDB collection based on the provided ID. The method takes an integer parameter representing the ID of the furniture to be retrieved and returns the corresponding Furniture object if found. If an error occurs during the retrieval process, it logs the error message and rethrows the exception.
/// </summary>
/// <param name="repo">The furniture service repository.</param>
/// <param name="id">The ID of the furniture to retrieve.</param>
/// <returns>The retrieved Furniture object, or null if not found.</returns>
        public async Task<Furniture> GetFurnitureById(
        [Service] FurnitureService repo, int id)
        {
            var Furniture = await repo.GetFurnitureById(id);return Furniture;
        }

        public async Task<Order> GetOrderById(
            [Service] OrderService repo,string id
        )
        {
            var order = await repo.GetOrderById(id);
            return order;
        }

        public async Task<List<EventEnvelope<Order>>> GetEnvelopes(
            [Service] EventEnvelopeService repo,string id
        )
        {
            var order = await repo.GetAllEnvelopes();
            return order;
        }

        public async Task<EventEnvelope<Order>> GetEventById(
            [Service] EventEnvelopeService repo,string id
        )
        {
            var envelope = await repo.GetEventById(id);
            return envelope;
        }

        
    }
}