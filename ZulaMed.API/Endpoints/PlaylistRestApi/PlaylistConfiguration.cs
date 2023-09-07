using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.Playlist;

namespace ZulaMed.API.Endpoints.PlaylistRestApi;

public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
{
    public void Configure(EntityTypeBuilder<Playlist> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasConversion<PlaylistId.EfCoreValueConverter>()
            .ValueGeneratedOnAdd()
            .IsRequired();
        
        builder.Property(x => x.PlaylistName)
            .HasConversion<PlaylistName.EfCoreValueConverter>()
            .IsRequired();
        
        builder.Property(x => x.PlaylistDescription)
            .HasConversion<PlaylistDescription.EfCoreValueConverter>()
            .IsRequired();
    }
}