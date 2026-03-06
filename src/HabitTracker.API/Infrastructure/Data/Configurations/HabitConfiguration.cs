using HabitTracker.API.Domain.Entities.HabitAggregate;
using HabitTracker.API.Domain.SeedWork;
using HabitTracker.API.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitTracker.API.Infrastructure.Data.Configurations;

public sealed class HabitConfiguration : IEntityTypeConfiguration<Habit>
{
    public void Configure(EntityTypeBuilder<Habit> builder)
    {
        builder.ToTable("Habits");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id).ValueGeneratedNever();

        // HabitName Value Object — owned
        builder.OwnsOne(h => h.Name, name =>
        {
            name.Property(n => n.Value)
                .HasColumnName("Name")
                .HasMaxLength(HabitName.MaxLength)
                .IsRequired();
        });

        // Frequency Value Object — owned
        builder.OwnsOne(h => h.Frequency, freq =>
        {
            freq.Property(f => f.Days)
                .HasColumnName("Frequency")
                .IsRequired();

            freq.Property(f => f.Type)
                .HasConversion(
                    t => t.Id,
                    id => Enumeration.FromValue<HabitFrequency>(id))
                .HasColumnName("FrequencyTypeId")
                .IsRequired();
        });

        builder.Property(h => h.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(h => h.CreatedAt)
            .IsRequired();

        // Child entities
        builder.HasMany(h => h.Logs)
            .WithOne()
            .HasForeignKey(l => l.HabitId)
            .OnDelete(DeleteBehavior.Cascade);

        // Never persist domain events
        builder.Ignore(h => h.DomainEvents);
    }
}