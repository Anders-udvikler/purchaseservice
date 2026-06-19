using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace models
{
public class EventEnvelope<T>
{
    [BsonId]
    public string eventId { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("eventType")]
    public string eventType { get; set; } = string.Empty;

    [BsonElement("eventVersion")]
    public int eventVersion { get; set; }

    [BsonElement("occurredAt")]
    public DateTime occurredAt { get; set; } = DateTime.UtcNow;

    [BsonElement("producer")]
    public string producer { get; set; } = string.Empty;

    [BsonElement("correlationId")]
    public string correlationId { get; set; } = string.Empty;

    [BsonElement("causationId")]
    public string causationId { get; set; } = string.Empty;

    [BsonElement("payload")]
    public T payload { get; set; } = default!;

    // =========================
    // OUTBOX ADDITIONS (IMPORTANT)
    // =========================

    [BsonElement("published")]
    public bool published { get; set; } = false;

    [BsonElement("publishedAt")]
    public DateTime? publishedAt { get; set; }

    [BsonElement("publishAttempts")]
    public int publishAttempts { get; set; } = 0;

    [BsonElement("lastPublishError")]
    public string? lastPublishError { get; set; }
}
}