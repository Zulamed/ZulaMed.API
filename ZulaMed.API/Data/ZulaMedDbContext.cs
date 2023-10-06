using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Domain.Comments;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Endpoints.Dislike;
using ZulaMed.API.Endpoints.Like;

namespace ZulaMed.API.Data;

public class ZulaMedDbContext : DbContext
{
    public ZulaMedDbContext(DbContextOptions<ZulaMedDbContext> options) : base(options){}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
        modelBuilder.ApplyConfiguration(new LikeConfiguration<Video>());
        modelBuilder.ApplyConfiguration(new DislikeConfiguration<Video>());
        
        modelBuilder.HasPostgresEnum<VideoStatus>();
    }
}