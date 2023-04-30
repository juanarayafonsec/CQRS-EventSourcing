using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Config;

namespace Post.Cmd.Infrastructure.Repositories;

public class EventStoreRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventStorCollection;

    public EventStoreRepository(IOptions<MongoDbConfig> config)
    {
        var mongoClient = new MongoClient(config.Value.Collection);
        var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);
        _eventStorCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);
    }

    public async Task SaveAsync(EventModel @event)
    {
        await _eventStorCollection.InsertOneAsync(@event).ConfigureAwait(false);
    }

    public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
    {
        return await _eventStorCollection.Find(x => x.AggregateIdentifier == aggregateId).ToListAsync()
            .ConfigureAwait(false);
    }
}