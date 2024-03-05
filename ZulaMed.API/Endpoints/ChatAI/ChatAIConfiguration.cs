using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.UserExamAI;

namespace ZulaMed.API.Endpoints.ChatAI;

public class ChatAIConfiguration: IEntityTypeConfiguration<UserExamAI>
{
    public void Configure(EntityTypeBuilder<UserExamAI> builder)
    {
        builder.HasKey(x => new {x.User.Id});

        //builder.HasOne(x => x.Subscriber)
         //   .WithMany(x => x.Subscriptions)
         //   .HasForeignKey(x => x.SubscriberId);

        //builder.HasOne(x => x.SubscribedTo)
        //    .WithMany(x => x.Subscribers)
        //    .HasForeignKey(x => x.SubscribedToId);
    }
}