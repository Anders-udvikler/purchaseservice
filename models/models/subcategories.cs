
using MongoDB.Bson.Serialization.Attributes;

namespace Subcategories
{
    [BsonIgnoreExtraElements]
    public class Subcategory
    {
        [BsonElement("id")]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set;}

        [BsonElement("categoryId")]
        public string? CategoryId { get; set; }
    }
}