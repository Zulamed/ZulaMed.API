using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.Accounts;
using ZulaMed.API.Domain.Accounts.HospitalAccount;

namespace ZulaMed.API.Endpoints.UserRestApi;

public class HospitalAccountConfiguration : IEntityTypeConfiguration<HospitalAccount>
{
    public void Configure(EntityTypeBuilder<HospitalAccount> builder)
    {
        builder.HasKey(x => x.User);
        builder.Property(x => x.AccountAddress)
            .HasConversion<AccountAddress.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.AccountPostCode)
            .HasConversion<AccountPostCode.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.AccountPhone)
            .HasConversion<AccountPhone.EfCoreValueConverter>()
            .IsRequired(); 
        builder.Property(x => x.AccountHospital)
            .HasConversion<AccountHospital.EfCoreValueConverter>()
            .IsRequired();
        
        
        builder.HasKey("UserId");
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey("UserId")
            .IsRequired();
    }
}