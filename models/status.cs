
using MongoDB.Bson.Serialization.Attributes;

namespace GraphProductView.Models
{
    public class Status
    {
        [BsonId]
        public string _Id { get; set; }

        [BsonElement("removed")]
        public bool Removed { get; set; }
        
        [BsonElement("date")]
        public DateTime? Date { get; set; }
    }
}