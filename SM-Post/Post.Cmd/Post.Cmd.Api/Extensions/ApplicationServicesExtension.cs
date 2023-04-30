using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Config;
using Post.Cmd.Infrastructure.Dispatchers;
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
        services.AddCommandHandlers();
    }

    private static void AddCommandHandlers(this IServiceCollection services)
    {
        var commandHandler = services.BuildServiceProvider().GetRequiredService<ICommandHandler>();
        
        var dispatcher = new CommandDispatcher();
        dispatcher.RegisterHandlder<NewPostCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandlder<EditMessageCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandlder<LikePostCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandlder<AddCommentCommands>(commandHandler.HandleAsync);
        dispatcher.RegisterHandlder<EditCommentCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandlder<RemoveCommentCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandlder<DeletePostCommand>(commandHandler.HandleAsync);
        
        services.AddSingleton<ICommandDispatcher>(_ => dispatcher);
    }
}