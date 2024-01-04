using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.Subscriptions;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.Id)
            .HasConversion<UserId.EfCoreValueConverter>()
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.Email)
            .HasConversion<UserEmail.EfCoreValueConverter>()
            .IsRequired();

        builder.Property(x => x.Login)
            .HasConversion<UserLogin.EfCoreValueConverter>()
            .IsRequired();

        builder
            .HasIndex(x => x.Login)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasConversion<UserName.EfCoreValueConverter>()
            .IsRequired();

        builder.Property(x => x.Surname)
            .HasConversion<UserSurname.EfCoreValueConverter>()
            .IsRequired();

        builder.Property(x => x.Country)
            .HasConversion<UserCountry.EfCoreValueConverter>()
            .IsRequired();

        builder.Property(x => x.City)
            .HasConversion<UserCity.EfCoreValueConverter>()
            .IsRequired();

        builder.Property(x => x.PhotoUrl)
            .HasConversion<PhotoUrl.EfCoreValueConverter>();

        builder.Property(x => x.HistoryPaused)
            .HasConversion<HistoryPaused.EfCoreValueConverter>();

        builder.Property(x => x.SubscriberCount)
            .HasConversion<SubscriberCount.EfCoreValueConverter>()
            .HasDefaultValue(SubscriberCount.Zero);

        builder.Property(x => x.Description)
            .HasConversion<Description.EfCoreValueConverter>();

        builder.Property(x => x.IsVerified)
            .HasConversion<IsVerified.EfCoreValueConverter>()
            .HasDefaultValue(IsVerified.From(false));

        builder.Property(x => x.RegistrationTime)
            .HasConversion<RegistrationTime.EfCoreValueConverter>();
        
        builder.Property(x => x.BannerUrl)
            .HasConversion<BannerUrl.EfCoreValueConverter>();
    }
}

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(x => new {x.SubscriberId, x.SubscribedToId});

        builder.HasOne(x => x.Subscriber)
            .WithMany(x => x.Subscriptions)
            .HasForeignKey(x => x.SubscriberId);

        builder.HasOne(x => x.SubscribedTo)
            .WithMany(x => x.Subscribers)
            .HasForeignKey(x => x.SubscribedToId);
    }
}