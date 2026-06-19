
using MongoDB.Bson.Serialization.Attributes;

namespace Images
{
    [BsonIgnoreExtraElements]
    public class Image
    {
        [BsonElement("id")]
        public string? Id { get; set; }

        [BsonElement("imageGuid")]
        public string? ImageGuid { get; set; }

        [BsonElement("url")]
        public string? Url { get; set; }
    }
}