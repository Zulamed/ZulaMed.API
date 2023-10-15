using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mux.Csharp.Sdk.Model;
using ZulaMed.API.Domain.LiveStream;
using LiveStream = ZulaMed.API.Domain.LiveStream.LiveStream;

namespace ZulaMed.API.Endpoints.MuxLiveStream;

public class LiveStreamConfiguration : IEntityTypeConfiguration<LiveStream>
{
    public void Configure(EntityTypeBuilder<LiveStream> builder)
    {
        builder.HasKey(x => x.Id);


        builder.Property(x => x.Id)
            .HasConversion<LiveStreamId.EfCoreValueConverter>()
            .ValueGeneratedOnAdd()
            .IsRequired();
        
        builder.Property(x => x.PlaybackId)
            .IsRequired();
        
        builder.Property(x => x.StreamKey)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasDefaultValue(LiveStreamStatus.Idle);

        builder.Property(x => x.MuxStreamId)
            .IsRequired();

    }
}