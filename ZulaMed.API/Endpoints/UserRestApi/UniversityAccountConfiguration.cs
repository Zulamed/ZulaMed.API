using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.Accounts;
using ZulaMed.API.Domain.Accounts.UniversityAccount;

namespace ZulaMed.API.Endpoints.UserRestApi;

public class UniversityAccountConfiguration : IEntityTypeConfiguration<UniversityAccount>
{
    public void Configure(EntityTypeBuilder<UniversityAccount> builder)
    {
        builder.HasKey("UserId");
        builder.Property(x => x.AccountAddress)
            .HasConversion<AccountAddress.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.AccountPostCode)
            .HasConversion<AccountPostCode.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.AccountPhone)
            .HasConversion<AccountPhone.EfCoreValueConverter>()
            .IsRequired();    
        builder.Property(x => x.AccountUniversity)
            .HasConversion<AccountUniversity.EfCoreValueConverter>()
            .IsRequired();
    }
}