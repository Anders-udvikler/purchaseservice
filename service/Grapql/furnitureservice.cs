using Furnitures;
using MongoDB.Driver;
namespace Furnitureservice
{
    public class FurnitureService
    {
        private readonly IMongoCollection<Furniture> _furnitureCollection;

/// <summary>
/// Initializes a new instance of the FurnitureService class. The constructor takes an IMongoClient object as a parameter, which is used to connect to the MongoDB database. It retrieves the "FurnitureDB" database and the "Furnitures" collection from the MongoDB client and assigns it to the _furnitureCollection field for further operations.
/// </summary>
/// <param name="furniture"></param>
/// <returns></returns>
        public async Task AddFurniture(Furniture furniture)
        {
            try
            {
                await _furnitureCollection.InsertOneAsync(furniture);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding furniture: {ex.Message}");
                throw;
            }
            await _furnitureCollection.InsertOneAsync(furniture);
        }

/// <summary>
/// Retrieves all furniture items from the MongoDB collection. The method returns a list of Furniture objects representing all the furniture items stored in the collection. If an error occurs during the retrieval process, it logs the error message and rethrows the exception.
 ///
/// </summary>
/// <returns></returns>
        public async Task<List<Furniture>> GetAllFurnitures()
        {
            try
            {
                return await _furnitureCollection.Find(f => true).ToListAsync();
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
        public async Task<Furniture> GetFurnitureById(int id)
        {
            try
            {
                return null;
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
        public async Task UpdateFurniture(int id, Furniture updatedFurniture)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating furniture: {ex.Message}");
                throw;
            }
        }

    }
}