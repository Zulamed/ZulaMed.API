using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.ViewHistory;

namespace ZulaMed.API.Endpoints.ViewHistory;

public class ViewHistoryConfiguration : IEntityTypeConfiguration<Domain.ViewHistory.ViewHistory>
{
    public void Configure(EntityTypeBuilder<Domain.ViewHistory.ViewHistory> builder)
    {
        //builder.HasKey("Id", "VideoId", "WatchedById");
        builder.HasKey("Id");
        builder.Property(x => x.ViewedAt)
            .HasConversion<ViewedAt.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.Id)
            .HasConversion<Id.EfCoreValueConverter>()
            .ValueGeneratedOnAdd()
            .IsRequired();
    }
}