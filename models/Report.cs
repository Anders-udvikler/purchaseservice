using Furnitures;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Users;
namespace Reports
{
    public class Report
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("furnitures")]
        public List<Furniture> Furnitures { get; set; }

        [BsonElement("user")]
        public User User { get; set; }
    }
}