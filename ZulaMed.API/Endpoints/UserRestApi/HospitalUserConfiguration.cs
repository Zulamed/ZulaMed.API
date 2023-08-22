using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.ExtendedUsers;
using ZulaMed.API.Domain.ExtendedUsers.HospitalUser;

namespace ZulaMed.API.Endpoints.UserRestApi;

public class HospitalUserConfiguration : IEntityTypeConfiguration<HospitalUser>
{
    public void Configure(EntityTypeBuilder<HospitalUser> builder)
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
        builder.Property(x => x.UserHospital)
            .HasConversion<UserHospital.EfCoreValueConverter>()
            .IsRequired();
        
        
        builder.HasKey("UserId");
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey("UserId")
            .IsRequired();
    }
}