using Microsoft.EntityFrameworkCore;

namespace ZulaMed.API.Data;

public class ZulaMedDbContext : DbContext
{
    public ZulaMedDbContext(DbContextOptions<ZulaMedDbContext> options) : base(options){}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
    }
}