using Microsoft.EntityFrameworkCore;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Api.Extensions;

public static class ApplicationConfigurationExtension
{
    public static void AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbConfiguration(config);
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