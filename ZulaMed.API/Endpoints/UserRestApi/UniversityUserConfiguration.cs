using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.UniversityUser;

namespace ZulaMed.API.Endpoints.UserRestApi;

public class UniversityUserConfiguration : IEntityTypeConfiguration<UniversityUser>
{
    public void Configure(EntityTypeBuilder<UniversityUser> builder)
    {
        builder.HasKey(x => x.User);
        builder.Property(x => x.UserAddress)
            .HasConversion<UserAddress.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.UserPostCode)
            .HasConversion<UserPostCode.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.UserPhone)
            .HasConversion<UserPhone.EfCoreValueConverter>()
            .IsRequired();
        
        
        builder.HasKey("UserId");
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey("UserId")
            .IsRequired();
    }
}