using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Console = RetroGaming.Domain.Entities.Console;

namespace RetroGaming.Infrastructure.Persistence.Configurations;

public class ConsoleConfiguration : IEntityTypeConfiguration<Console>
{
    public void Configure(EntityTypeBuilder<Console> builder)
    {
        builder.ToTable("Consoles");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ManufacturerId)
            .IsRequired();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.ReleaseYear)
            .IsRequired();

        builder.Property(c => c.Generation)
            .IsRequired();

        builder.Property(c => c.UnitsSoldInMillions)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.IsDiscontinued)
            .IsRequired();

        builder.HasIndex(c => new { c.ManufacturerId, c.Name })
            .IsUnique();

        builder.HasOne(c => c.Manufacturer)
            .WithMany(m => m.Consoles)
            .HasForeignKey(c => c.ManufacturerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
