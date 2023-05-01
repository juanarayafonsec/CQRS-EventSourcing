using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repository;
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