using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.Accounts;
using ZulaMed.API.Domain.Accounts.PersonalAccount;

namespace ZulaMed.API.Endpoints.UserRestApi;

public class PersonalAccountConfiguration : IEntityTypeConfiguration<PersonalAccount>
{
    public void Configure(EntityTypeBuilder<PersonalAccount> builder)
    {
        builder.HasKey(x => x.User);
        builder.Property(x => x.AccountDepartment)
            .HasConversion<AccountDepartment.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.AccountRole)
            .HasConversion<AccountRole.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.AccountCareerStage)
            .HasConversion<AccountCareerStage.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.AccountGender)
            .HasConversion<AccountGender.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.AccountTitle)
            .HasConversion<AccountTitle.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.AccountSpecialty)
            .HasConversion<AccountSpecialty.EfCoreValueConverter>()
            .IsRequired();
        builder.Property(x => x.AccountProfessionalActivity)
            .HasConversion<AccountProfessionalActivity.EfCoreValueConverter>()
            .IsRequired();


        builder.HasKey("UserId");
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey("UserId")
            .IsRequired();
    }
}