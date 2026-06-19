using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Purchase.Models;

public class Purchase
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement]
    public string PaymentsProvider { get; set; } = string.Empty;

    [BsonElement]
    public string PaymentStatus { get; set; } = string.Empty;
    [BsonElement]
    public decimal Amount { get; set; }
    [BsonElement]
    public string Currency { get; set; } = "DKK";
    [BsonElement]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [BsonElement]
    public DateTime? ProcessedAt { get; set; }
    [BsonElement]
    public Order Order { get; set; } = new();
}