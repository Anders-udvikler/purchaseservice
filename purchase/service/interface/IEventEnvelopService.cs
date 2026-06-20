using models;

namespace service.Grapql;

public interface IEventEnvelopeService<T>
{
    Task AddEvent(EventEnvelope<T> envelope);
    Task<List<EventEnvelope<T>>> GetAllEnvelopes();
    Task<EventEnvelope<T>?> GetEventById(string id);
    Task UpdateEvent(string id, EventEnvelope<T> envelope);

    Task<List<EventEnvelope<T>>> GetUnpublishedEvents();
    Task MarkPublished(string eventId);
}