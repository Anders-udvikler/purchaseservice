using DTO;
using MongoDB.Driver;
namespace service {


public class ProcessedEventService
{
    private readonly IMongoCollection<ProcessedEvents> _collection;

    public ProcessedEventService(IMongoDatabase db)
    {
        _collection = db.GetCollection<ProcessedEvents>("processed_events");
    }

    public async Task<bool> AlreadyProcessed(string eventId)
    {
        return await _collection
            .Find(x => x.EventId == eventId)
            .AnyAsync();
    }

    public async Task MarkProcessed(string eventId)
    {
        await _collection.InsertOneAsync(new ProcessedEvents
        {
            EventId = eventId,
            ProcessedAt = DateTime.UtcNow
        });
    }
}
}