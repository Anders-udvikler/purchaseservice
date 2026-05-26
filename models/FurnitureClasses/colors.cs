
using MongoDB.Bson.Serialization.Attributes;

namespace Colors
{
    public class Color
    {
        [BsonId]                  // optional, if _id exists
        public string _Id { get; set; }


        [BsonElement("id")]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("href")]
        public string? Href { get; set; }
    }
}