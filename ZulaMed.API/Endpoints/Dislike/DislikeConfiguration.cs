using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.Dislike;
using ZulaMed.API.Domain.Shared;

namespace ZulaMed.API.Endpoints.Dislike;

public class DislikeConfiguration<T> : IEntityTypeConfiguration<Dislike<T>>
{
    public void Configure(EntityTypeBuilder<Dislike<T>> builder)
    {
        builder.HasKey("Id", "ParentId", "DislikedById");
        builder.Property(x => x.DislikedAt)
            .HasConversion<DislikedAt.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.Id)
            .HasConversion<Id.EfCoreValueConverter>()
            .ValueGeneratedOnAdd()
            .IsRequired();
    }
}