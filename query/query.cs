using Furnitureservice;
using HotChocolate;
using Furnitures;

namespace query
{
    public class Query
    {
        public async Task<List<Furniture>> GetAllFurniture(
        [Service] FurnitureService repo)
        {
        var allFurniture = await repo.GetAllFurnitures();
        return allFurniture;
        }

        public async Task<Furniture> GetFurnitureById(
        [Service] FurnitureService repo)
        {
            var Furniture = await repo.GetFurnitureById(1);return Furniture;
        }
    }
}