using Furnitureservice;
using HotChocolate;
using Furnitures;
using Reports;
using UserService;

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

/// <summary>
/// Retrieves all reports from the MongoDB collection. The method returns a list of Report objects representing all the reports stored in the collection. If an error occurs during the retrieval process, it logs the error message and rethrows the exception.
/// </summary>
/// <param name="repo">The report service repository.</param>
/// <returns>A list of all reports.</returns>
        public async Task<List<Report>> GetAllReports(
        [Service] ReportService repo)
        {
            var allReports = await repo.GetAllReports();
            return allReports;
        }


/// <summary>
///     Retrieves a report from the MongoDB collection based on the provided ID. The method takes a string parameter representing the ID of the report to be retrieved and returns the corresponding Report object if found. If an error occurs during the retrieval process, it logs the error message and rethrows the exception.
/// </summary>
/// <param name="repo">The report service repository.</param>
/// <param name="id">The ID of the report to retrieve.</param>
/// <returns>The retrieved Report object, or null if not found.</returns>
        public async Task<Report> GetReportById(
        [Service] ReportService repo, string id)
        {
            var report = await repo.GetReportById(id);
            return report;
        }
    }
}