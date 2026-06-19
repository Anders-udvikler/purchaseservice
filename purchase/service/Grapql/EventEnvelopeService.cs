using Furnitures;
using models;
using MongoDB.Driver;
using Purchase.Models;
using Stripe;
namespace service.Grapql
{

    public class EventEnvelopeService<T>
    {
        private readonly IMongoCollection<EventEnvelope<T>> _eventCollection;

/// <summary>
/// Initializes a new instance of the FurnitureService class. The constructor takes an IMongoClient object as a parameter, which is used to connect to the MongoDB database. It retrieves the "FurnitureDB" database and the "Furnitures" collection from the MongoDB client and assigns it to the _furnitureCollection field for further operations.
/// </summary>
/// <param name="furniture"></param>
/// <returns></returns>
        public async Task Addevent(EventEnvelope<T> envelope)
        {
            try
            {
                
                await _eventCollection.InsertOneAsync(envelope);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding furniture: {ex.Message}");
                throw;
            }
            await _eventCollection.InsertOneAsync(envelope);
        }

/// <summary>
/// Retrieves all furniture items from the MongoDB collection. The method returns a list of Furniture objects representing all the furniture items stored in the collection. If an error occurs during the retrieval process, it logs the error message and rethrows the exception.
 ///
/// </summary>
/// <returns></returns>
        public async Task<List<EventEnvelope<T>>> GetAllEnvelopes()
        {
            try
            {
                return await _eventCollection.Find(f => true).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching furnitures: {ex.Message}");
                throw;
            }
        }

/// <summary>
/// Retrieves a furniture item from the MongoDB collection based on the provided ID. The method takes an integer parameter representing the ID of the furniture to be retrieved and returns the corresponding Furniture object if found. If an error occurs during the retrieval process, it logs the error message and rethrows the exception.
/// </summary>
/// <param name="id">The ID of the furniture to retrieve.</param>
/// <returns>The retrieved Furniture object, or null if not found.</returns>
        public async Task<EventEnvelope<T>> GetEventById(string id)
        {
            try
            {
                return await _eventCollection.Find(i => i.eventId == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching furniture: {ex.Message}");
                throw;
            }
        }

/// <summary>
/// Updates an existing furniture item in the MongoDB collection. The method takes an integer ID representing the furniture to be updated and a Furniture object containing the updated information. It uses the ReplaceOneAsync method to replace the existing furniture document with the updated one based on the provided ID. If an error occurs during the update process, it logs the error message and rethrows the exception.
/// </summary>
/// <param name="id">The ID of the furniture to update.</param>
/// <param name="updatedFurniture">The updated furniture object.</param>
/// <returns>The updated Furniture object.</returns>
public async Task UpdateFurniture(string id, EventEnvelope<T> furniture)
{
    try
    {
        var filter = Builders<EventEnvelope<T>>.Filter.Eq(x => x.eventId, id);

        await _eventCollection.ReplaceOneAsync(
            filter,
            furniture
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating furniture: {ex.Message}");
        throw;
    }
}

        public async Task<List<EventEnvelope<T>>> GetUnpublishedEvents()
        {
                return
                 await _eventCollection.Find(x => !x.published).ToListAsync();
        }

        public async Task MarkPublished(string eventid)
        {
                var update = Builders<EventEnvelope<T>>
                .Update
                .Set(x => x.published, true)
                .Set(x => x.publishedAt, DateTime.UtcNow);
                await _eventCollection.UpdateOneAsync(
            x => x.eventId == eventid,update);
        }

    }
}
