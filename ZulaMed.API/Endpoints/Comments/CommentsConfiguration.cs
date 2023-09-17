using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZulaMed.API.Domain.Comments;
using ZulaMed.API.Domain.Shared;

namespace ZulaMed.API.Endpoints.Comments;

public class CommentsConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion<Id.EfCoreValueConverter>()
            .IsRequired();

        builder.Property(x => x.Content)
            .HasConversion<CommentContent.EfCoreValueConverter>();

        builder.Property(x => x.SentAt)
            .HasConversion<CommentSentDate.EfCoreValueConverter>()
            .IsRequired();
    }
}

public class ReplyConfiguration : IEntityTypeConfiguration<Reply>
{
    public void Configure(EntityTypeBuilder<Reply> builder)
    {
        builder.HasKey("ParentCommentId", "ReplyCommentId");
        
        builder.HasOne(x => x.ParentComment)
            .WithMany()
            .HasForeignKey("ParentCommentId")
            .IsRequired();

        builder.HasOne(x => x.ReplyComment)
            .WithMany()
            .HasForeignKey("ReplyCommentId")
            .IsRequired();
    }
}