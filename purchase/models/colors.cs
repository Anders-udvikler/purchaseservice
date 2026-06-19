
using MongoDB.Bson.Serialization.Attributes;

namespace Colors
{
    [BsonIgnoreExtraElements]
    public class Color
    {
        [BsonElement("id")]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("href")]
        public string? Href { get; set; }
    }
}