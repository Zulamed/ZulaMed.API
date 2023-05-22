using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.Video;

public class VideoConfiguration : IEntityTypeConfiguration<Domain.Video.Video>
{
    public void Configure(EntityTypeBuilder<Domain.Video.Video> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion<VideoId.EfCoreValueConverter>()
            .ValueGeneratedOnAdd()
            .IsRequired();
        builder.Property(x => x.VideoUrl)
            .HasConversion<VideoUrl.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.VideoDescription)
            .HasConversion<VideoDescription.EfCoreValueConverter>();
        builder.Property(x => x.VideoPublishedDate)
            .HasConversion<VideoPublishedDate.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.VideoThumbnail)
            .HasConversion<VideoThumbnail.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.VideoTitle)
            .HasConversion<VideoTitle.EfCoreValueConverter>()
            .IsRequired();
    }
}