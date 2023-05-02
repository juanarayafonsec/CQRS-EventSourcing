using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Config;
using Post.Cmd.Infrastructure.Dispatchers;
using Post.Cmd.Infrastructure.Handlers;
using Post.Cmd.Infrastructure.Producers;
using Post.Cmd.Infrastructure.Repositories;
using Post.Cmd.Infrastructure.Stores;

namespace Post.Cmd.Api.Extensions;

public static class ApplicationServicesExtension
{
    public static void AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<MongoDbConfig>(config.GetSection(nameof(MongoDbConfig)));
        services.Configure<ProducerConfig>(config.GetSection(nameof(ProducerConfig)));
        
        services.AddScoped<IEventStoreRepository, EventStoreRepository>();
        services.AddScoped<IEventProducer, EventProducer>();
        services.AddScoped<IEventStore, EventStore>();
        services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
        services.AddScoped<ICommandHandler, CommandHandler>();
        services.AddCommandHandlers();
    }

    private static void AddCommandHandlers(this IServiceCollection services)
    {
        var commandHandler = services.BuildServiceProvider().GetRequiredService<ICommandHandler>();
        
        var dispatcher = new CommandDispatcher();
        dispatcher.RegisterHandler<NewPostCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<EditMessageCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<LikePostCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<AddCommentCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<EditCommentCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<RemoveCommentCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<DeletePostCommand>(commandHandler.HandleAsync);
        
        services.AddSingleton<ICommandDispatcher>(_ => dispatcher);
    }
}