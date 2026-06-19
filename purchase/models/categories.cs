using Subcategories;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Categories
{
    [BsonIgnoreExtraElements]
    public class Category
    {
        [BsonElement("id")]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set;}

        [BsonElement("subcats")]
        public List<Subcategory>? Subcats { get; set; }
    }
}