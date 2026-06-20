namespace service;

public interface IProcessedEventService
{
    Task<bool> AlreadyProcessed(string eventId);
    Task MarkProcessed(string eventId);
}