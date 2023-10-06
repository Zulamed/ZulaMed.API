using Microsoft.EntityFrameworkCore;
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
        
        var dataSource = sourceBuilder.Build();

        services.AddDbContext<ZulaMedDbContext>(options => { options.UseNpgsql(dataSource); });
    }
}