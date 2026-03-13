using HabitTracker.API.Domain.Entities.UserAggregate;
using HabitTracker.API.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitTracker.API.Infrastructure.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        // UserId Value Object conversion
        builder.Property(u => u.Id).ValueGeneratedNever();

        // UserName Value Object — owned
        builder.OwnsOne(u => u.Name, name =>
        {
            name.Property(n => n.Value)
                .HasColumnName("Name")
                .HasMaxLength(UserName.MaxLength)
                .IsRequired();
        });

        // Email Value Object — owned
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(Email.MaxLength)
                .IsRequired();
        });

        builder.Property<string>("PasswordHash")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        // Unique email index
        builder.HasIndex("Email_Value")
            .IsUnique();

        // Never persist domain events
        builder.Ignore(u => u.DomainEvents);
    }
}