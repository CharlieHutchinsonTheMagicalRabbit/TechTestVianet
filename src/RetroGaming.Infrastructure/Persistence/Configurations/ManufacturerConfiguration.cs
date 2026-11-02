using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetroGaming.Domain.Entities;

namespace RetroGaming.Infrastructure.Persistence.Configurations;

public class ManufacturerConfiguration : IEntityTypeConfiguration<Manufacturer>
{
    public void Configure(EntityTypeBuilder<Manufacturer> builder)
    {
        builder.ToTable("Manufacturers");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(m => m.HqCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.HqCountry)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.FoundedYear)
            .IsRequired();

        builder.Property(m => m.Latitude)
            .IsRequired();

        builder.Property(m => m.Longitude)
            .IsRequired();

        builder.HasIndex(m => m.Name);

        builder.HasMany(m => m.Consoles)
            .WithOne(c => c.Manufacturer)
            .HasForeignKey(c => c.ManufacturerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(m => m.Consoles)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
