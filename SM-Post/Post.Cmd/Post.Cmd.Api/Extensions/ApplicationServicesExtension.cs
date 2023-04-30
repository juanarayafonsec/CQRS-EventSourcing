using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Config;
using Post.Cmd.Infrastructure.Handlers;
using Post.Cmd.Infrastructure.Repositories;
using Post.Cmd.Infrastructure.Stores;

namespace Post.Cmd.Api.Extensions;

public static class ApplicationServicesExtension
{
    public static void AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<MongoDbConfig>(config.GetSection(nameof(MongoDbConfig)));
        services.AddScoped<IEventStoreRepository, EventStoreRepository>();
        services.AddScoped<IEventStore, EventStore>();
        services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
        services.AddScoped<ICommandHandler, CommandHandler>();
    }
}