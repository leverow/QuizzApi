using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace quizz.Entities.Configurations;

public class TestCaseConfiguration<T> : IEntityTypeConfiguration<T> where T : TestCase
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd()
            .HasAnnotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.SerialColumn);
        builder.HasKey(t => new { t.Id, t.QuestionId });
        builder.Property(t => t.FileName).IsRequired();
        builder.Property(t => t.QuestionId).IsRequired();
    }
}