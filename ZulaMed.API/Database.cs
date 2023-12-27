using Microsoft.EntityFrameworkCore;
using Mux.Csharp.Sdk.Model;
using Npgsql;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API;

public static class Database
{
    public static void AddDatabase(this IServiceCollection services, string? connectionString)
    {
        var sourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

        sourceBuilder.MapEnum<VideoStatus>();
        sourceBuilder.MapEnum<LiveStreamStatus>();
        
        var dataSource = sourceBuilder.Build();

        services.AddDbContext<ZulaMedDbContext>(options => options.UseNpgsql(dataSource), optionsLifetime: ServiceLifetime.Singleton);
        services.AddDbContextFactory<ZulaMedDbContext>(options => options.UseNpgsql(dataSource));
    }
}