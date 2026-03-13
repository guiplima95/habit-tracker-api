using HabitTracker.API.Domain.Entities.HabitAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitTracker.API.Infrastructure.Data.Configurations;

public sealed class HabitLogConfiguration : IEntityTypeConfiguration<HabitLog>
{
    public void Configure(EntityTypeBuilder<HabitLog> builder)
    {
        builder.ToTable("HabitLogs");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.HabitId)
            .IsRequired();

        builder.Property(l => l.CompletedAt)
            .IsRequired();

        builder.Property(l => l.Notes)
            .HasMaxLength(500)
            .IsRequired(false);
    }
}