using MongoDB.Bson.Serialization.Attributes;

namespace DTO
{
    public class ProcessedEvents
    {
        [BsonId]
    public string EventId { get; set; } = string.Empty;

    [BsonElement("processedAt")]
    public DateTime ProcessedAt { get; set; }
    }
}