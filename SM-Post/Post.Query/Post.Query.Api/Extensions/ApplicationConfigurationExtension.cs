using Confluent.Kafka;
using CQRS.Core.Consumers;
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repository;
using EventConsumer = Post.Query.Infrastructure.Consumers.EventConsumer;
using EventHandler = Post.Query.Infrastructure.Handlers.EventHandler;

namespace Post.Query.Api.Extensions;

public static class ApplicationConfigurationExtension
{
    public static void AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbConfiguration(config);
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IEventHandler, EventHandler>();
        services.Configure<ConsumerConfig>(config.GetSection(nameof(ConsumerConfig)));
        services.AddScoped<IEventConsumer, EventConsumer>();
        services.AddHostedService<ConsumerHostedService>();
    }

    private static void AddDbConfiguration(this IServiceCollection services, IConfiguration config)
    {
        Action<DbContextOptionsBuilder> configureDbContext =
            o => o.UseLazyLoadingProxies().UseSqlServer(config.GetConnectionString("SqlServer"));
        services.AddDbContext<DatabaseContext>(configureDbContext);
        services.AddSingleton(new DatabaseContextFactory(configureDbContext));
    }

    public static void CreateDbAndTables(this IServiceCollection services)
    {
        var dataContext = services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
        dataContext.Database.EnsureCreated();
    }
}