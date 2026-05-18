
using MongoDB.Bson.Serialization.Attributes;

namespace Images
{
    public class Image
    {
        [BsonId]                  // optional, if _id exists
        public string _Id { get; set; }

        [BsonElement("id")]
        public string? Id { get; set; }


        [BsonElement("url")]
        public string? url { get; set; }
    }
}