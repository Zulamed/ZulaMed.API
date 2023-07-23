using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.Comments;

namespace ZulaMed.API.Endpoints.Comments;

public class CommentsConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion<CommentId.EfCoreValueConverter>()
            .IsRequired();

        builder.Property(x => x.Content)
            .HasConversion<CommentContent.EfCoreValueConverter>();

        builder.Property(x => x.Dislike)
            .HasConversion<Dislike.EfCoreValueConverter>()
            .HasDefaultValue(Dislike.Zero);

        builder.Property(x => x.Like)
            .HasConversion<Like.EfCoreValueConverter>()
            .HasDefaultValue(Like.Zero);

        builder.Property(x => x.SentAt)
            .HasConversion<CommentSentDate.EfCoreValueConverter>()
            .IsRequired();
    }
}

public class ReplyConfiguration : IEntityTypeConfiguration<Reply>
{
    public void Configure(EntityTypeBuilder<Reply> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion<ReplyId.EfCoreValueConverter>()
            .IsRequired();
    }
}