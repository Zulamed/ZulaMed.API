using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi;

public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
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
        builder.Property(x => x.VideoLike)
            .HasConversion<VideoLike.EfCoreValueConverter>()
            .HasDefaultValue(VideoLike.Zero);
        builder.Property(x => x.VideoDislike)
            .HasConversion<VideoDislike.EfCoreValueConverter>()
            .HasDefaultValue(VideoDislike.Zero);
        builder.Property(x => x.VideoView)
            .HasConversion<VideoView.EfCoreValueConverter>()
            .HasDefaultValue(VideoView.Zero);
        builder.Property(x => x.VideoPublisherId)
            .IsRequired()
            .HasConversion<VideoPublisherId.EfCoreValueConverter>();
    }
}