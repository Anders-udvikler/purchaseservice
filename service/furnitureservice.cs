using Furnitures;
using MongoDB.Driver;
namespace Furnitureservice
{
    public class FurnitureService
    {
        private readonly IMongoCollection<Furniture> _furnitureCollection;

        public async Task AddFurniture(Furniture furniture)
        {
            await _furnitureCollection.InsertOneAsync(furniture);
        }

        public async Task<List<Furniture>> GetAllFurnitures()
        {
            return await _furnitureCollection.Find(f => true).ToListAsync();
        }

        public async Task<Furniture> GetFurnitureById(int id)
        {
            return await _furnitureCollection.Find(f => f.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task UpdateFurniture(int id, Furniture updatedFurniture)
        {
            await _furnitureCollection.ReplaceOneAsync(f => f.Id.Equals(id), updatedFurniture);
        }

    }
}