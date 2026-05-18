    using Users;
    using MongoDB.Driver;
using Reports;
namespace UserService
{


    public class ReportService
    {
        private readonly IMongoCollection<Report> _ReportCollection;

        public async Task AddUser(Report user)
        {
            await _ReportCollection.InsertOneAsync(user);
        }
    }
}