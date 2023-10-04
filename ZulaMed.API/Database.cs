using Microsoft.EntityFrameworkCore;
using Npgsql;
using ZulaMed.API.Data;

namespace ZulaMed.API;

public static class Database
{
    public static void AddDatabase(this IServiceCollection services, string? connectionString)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ZulaMedDbContext>(options => { options.UseNpgsql(dataSource); });
    }
}