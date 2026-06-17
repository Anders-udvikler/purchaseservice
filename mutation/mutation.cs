using Furnitureservice;
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

        //furniture mutations


/// <summary>
/// Adds a new furniture item to the MongoDB collection. The method takes a Furniture object as a parameter and inserts it into the collection. If the insertion is successful, it returns the added Furniture object. If an error occurs during the insertion process, it logs the error message and rethrows the exception.
/// </summary>
/// <param name="repo">The furniture service repository.</param>
/// <param name="furniture">The furniture item to add.</param>
/// <returns>The added Furniture object, or null if the operation fails.</returns>
        public async Task<Furniture> AddFurniture(
        [Service] FurnitureService repo, Furniture furniture)
        {
            try
            {
                await repo.AddFurniture(furniture);
                return furniture;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding furniture: {ex.Message}");
                return null;
            }
        }



        //report mutations

/// <summary>
/// Adds a new report to the MongoDB collection. The method takes a Report object as a parameter and inserts it into the collection. If the insertion is successful, it returns the added Report object. If an error occurs during the insertion process, it logs the error message and rethrows the exception.
/// </summary>
/// <param name="repo">The report service repository.</param>
/// <param name="report">The report to add.</param>
/// <returns>The added Report object, or null if the operation fails.</returns>
        public async Task<Order> AddOrder(
        [Service] OrderService repo, Order order)
        {
            try
            {
                return await repo.AddOrder(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding report: {ex.Message}");
                return null;
            }
        }

/// <summary>
/// Deletes a report from the MongoDB collection. The method takes a Report object as a parameter and uses the DeleteReport method of the ReportService to perform the deletion based on the report's ID. If an error occurs during the deletion process, it logs the error message.
/// </summary>
/// <param name="repo">The report service repository.</param>
/// <param name="report">The report to delete.</param>
/// <returns>A task representing the asynchronous operation.</returns>
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

        public async Task AddEnvelope(
            [Service] EventEnvelopeService repo,EventEnvelope<Order> envelope)
        {
            await repo.Addevent(envelope);
        }
}}