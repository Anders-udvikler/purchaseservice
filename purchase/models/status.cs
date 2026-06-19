
using MongoDB.Bson.Serialization.Attributes;

namespace GraphProductView.Models
{
    [BsonIgnoreExtraElements]
    public class Status
    {
        [BsonElement("id")]
        public string? Id { get; set; }

        [BsonElement("removed")]
        public bool Removed { get; set; }
        
        [BsonElement("date")]
        public DateTime? Date { get; set; }
    }
}