using MongoDB.Bson.Serialization.Attributes;

namespace GraphProductView.Models
{
    public class Sales_Post_Removed
    {
        [BsonElement("id")]
        public string? Id { get; set; }

        [BsonElement("productId")]
        public string? ProductId { get; set; }

        [BsonElement("removed")]
        public bool Removed { get; set; }
        [BsonElement("date")]
        public DateTime? Date { get; set; }
    }
}   