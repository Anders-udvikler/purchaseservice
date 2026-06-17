using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace models
{
    public class EventEnvelope<T>
    {
        [BsonId]
        public string eventId { get; set; } = string.Empty;

        [BsonElement("eventType")]
        public string eventType { get; set; } = string.Empty;

        [BsonElement("eventVersion")]
        public int eventVersion { get; set; }

        [BsonElement("occurredAt")]
        public DateTime occurredAt { get; set; }

        [BsonElement("producer")]
        public string producer { get; set; } = string.Empty;

        [BsonElement("correlationId")]
        public string correlationId { get; set; } = string.Empty;

        [BsonElement("causationId")]
        public string causationId { get; set; } = string.Empty;

        [BsonElement("payload")]
        public T payload { get; set; } = default!;
    }
}