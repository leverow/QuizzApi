using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace quizz.Entities.Configurations;

public class TopicConfiguration : EntityBaseConfiguration<Topic>
{
    public override void Configure(EntityTypeBuilder<Topic> builder)
    {
        base.Configure(builder);

        builder.HasIndex(p => p.NameHash).IsUnique();
        builder.Property(p => p.Name).HasMaxLength(50).IsRequired();
        builder.Property(p => p.NameHash).HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(p => p.Description).HasMaxLength(255).IsRequired();
    }
}