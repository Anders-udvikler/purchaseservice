
using MongoDB.Bson.Serialization.Attributes;

namespace Subcategories
{
    public class Subcategory
    {
        [BsonId]                  // optional, if _id exists
        public string _Id { get; set; }

        [BsonElement("id")]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set;}
    }
}