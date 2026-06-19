using Furnitures;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Purchase.Enums;

namespace Purchase.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userGuid")]
    public string UserGuid { get; set; } = string.Empty;

    
    [BsonElement("email")]
    public string email { get; set; } = string.Empty;

    [BsonElement("orderStatus")]
    public OrderStatus OrderStatus { get; set; }

    [BsonElement("orderItems")]
    public List<Furniture> OrderItems { get; set; } = new();

    [BsonElement("totalPrice")]
    public decimal TotalPrice { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}