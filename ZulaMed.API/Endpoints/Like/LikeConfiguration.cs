using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.Like;

namespace ZulaMed.API.Endpoints.Like;

public class LikeConfiguration<T> : IEntityTypeConfiguration<Like<T>>
{
    public void Configure(EntityTypeBuilder<Like<T>> builder)
    {
        builder.HasKey("Id", "ParentId", "LikedAt");
        builder.Property(x => x.LikedAt)
            .HasConversion<LikedAt.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.Id)
            .HasConversion<Id.EfCoreValueConverter>()
            .ValueGeneratedOnAdd()
            .IsRequired();
    }
}