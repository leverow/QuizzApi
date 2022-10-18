using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace quizz.Entities.Configurations;

public class QuizConfiguration : EntityBaseConfiguration<Quiz>
{
    public override void Configure(EntityTypeBuilder<Quiz> builder)
    {
        base.Configure(builder);

        builder.Property(b => b.Title).HasMaxLength(255).IsRequired(true);
        builder.Property(b => b.Description).IsRequired(true);
        builder.Property(b => b.StartTime).IsRequired(true);
        builder.Property(b => b.EndTime).IsRequired(true);
        builder.Property(b => b.PasswordHash).HasMaxLength(64).IsFixedLength().IsRequired();
    }
}