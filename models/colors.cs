
using MongoDB.Bson.Serialization.Attributes;

namespace Colors
{
    public class Color
    {
        [BsonId]                  // optional, if _id exists
        public string _Id { get; set; }


        [BsonElement("id")]
        public string? id { get; set; }

        [BsonElement("name")]
        public string? name { get; set; }

        [BsonElement("href")]
        public string? href { get; set; }
    }
}