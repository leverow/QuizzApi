using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace quizz.Entities.Configurations;

public class QuestionConfiguration : EntityBaseConfiguration<Question>
{
    public override void Configure(EntityTypeBuilder<Question> builder)
    {
        base.Configure(builder);

        builder.Property(b => b.TopicId).IsRequired(true);
        builder.Property(b => b.Title).HasMaxLength(255).IsRequired(true);
        builder.Property(b => b.Description).IsRequired(true);
        builder.Property(b => b.TimeAllowed).IsRequired();
    }
}