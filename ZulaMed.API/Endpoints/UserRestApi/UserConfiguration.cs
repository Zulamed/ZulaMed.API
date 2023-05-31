using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.SpecialtyGroup;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi;

public class UserConfiguration :IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion<UserId.EfCoreValueConverter>()
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.Email)
            .HasConversion<UserEmail>()
            .IsRequired();

        builder.HasOne(x => x.Group)
            .WithMany();
        
        builder.Property(x => x.Name)
            .HasConversion<UserName>()
            .IsRequired();

        builder.Property(x => x.Surname)
            .HasConversion<UserSurname>()
            .IsRequired();
        
        builder.Property(x => x.Country)
            .HasConversion<UserCountry>()
            .IsRequired();

        builder.Property(x => x.City)
            .HasConversion<UserCity>()
            .IsRequired();

        builder.Property(x => x.University)
            .HasConversion<UserUniversity>();

        builder.Property(x => x.WorkPlace)
            .HasConversion<UserWorkPlace>();
    }
}


public class SpecialtyGroupConfiguration : IEntityTypeConfiguration<SpecialtyGroup>
{
    public void Configure(EntityTypeBuilder<SpecialtyGroup> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion<SpecialtyGroupId.EfCoreValueConverter>()
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.Name)
            .HasConversion<SpecialtyGroupName.EfCoreValueConverter>()
            .IsRequired();
    }
}