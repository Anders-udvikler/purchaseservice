using Furnitureservice;
using HotChocolate;
using Furnitures;

namespace mutation
{
    public class Mutation
    {

        public async Task<List<Furniture>> GetAllFurniture(
        [Service] FurnitureService repo)
        {
            return await repo.GetAllFurnitures();
        }

        public async Task<Furniture> GetFurnitureById(
        [Service] FurnitureService repo)
        {
            var Furniture = await repo.GetFurnitureById(1);return Furniture;
        }
    }
}