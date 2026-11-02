using Microsoft.EntityFrameworkCore;
using RetroGaming.Domain.Entities;
using Console = RetroGaming.Domain.Entities.Console;

namespace RetroGaming.Infrastructure.Persistence;

public static class ModelBuilderExtensions
{
    public static readonly Guid NintendoId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid SegaId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid SonyId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid AtariId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid MicrosoftId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    public static readonly Guid NesId = Guid.Parse("a1111111-1111-1111-1111-111111111111");
    public static readonly Guid SnesId = Guid.Parse("a2222222-2222-2222-2222-222222222222");
    public static readonly Guid N64Id = Guid.Parse("a3333333-3333-3333-3333-333333333333");

    public static readonly Guid MasterSystemId = Guid.Parse("b1111111-1111-1111-1111-111111111111");
    public static readonly Guid MegaDriveId = Guid.Parse("b2222222-2222-2222-2222-222222222222");
    public static readonly Guid DreamcastId = Guid.Parse("b3333333-3333-3333-3333-333333333333");

    public static readonly Guid PlayStation1Id = Guid.Parse("c1111111-1111-1111-1111-111111111111");
    public static readonly Guid PlayStation2Id = Guid.Parse("c2222222-2222-2222-2222-222222222222");

    public static readonly Guid Atari2600Id = Guid.Parse("d1111111-1111-1111-1111-111111111111");

    public static readonly Guid XboxId = Guid.Parse("e1111111-1111-1111-1111-111111111111");

    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manufacturer>().HasData(
            new Manufacturer(
                NintendoId,
                "Nintendo",
                "Kyoto",
                "Japan",
                1889,
                34.9699,
                135.7563),
            new Manufacturer(
                SegaId,
                "Sega",
                "Tokyo",
                "Japan",
                1960,
                35.6264,
                139.7402),
            new Manufacturer(
                SonyId,
                "Sony Interactive Entertainment",
                "San Mateo",
                "USA",
                1993,
                37.5485,
                -122.3131),
            new Manufacturer(
                AtariId,
                "Atari",
                "Sunnyvale",
                "USA",
                1972,
                37.3688,
                -122.0363),
            new Manufacturer(
                MicrosoftId,
                "Microsoft",
                "Redmond",
                "USA",
                1975,
                47.6423,
                -122.1391)
        );

        modelBuilder.Entity<Console>().HasData(
            new Console(
                NesId,
                NintendoId,
                "NES / Famicom",
                1983,
                3,
                61.91m,
                true),
            new Console(
                SnesId,
                NintendoId,
                "Super Nintendo",
                1990,
                4,
                49.10m,
                true),
            new Console(
                N64Id,
                NintendoId,
                "N64",
                1996,
                5,
                32.93m,
                true),
            new Console(
                MasterSystemId,
                SegaId,
                "Master System",
                1985,
                3,
                13.00m,
                true),
            new Console(
                MegaDriveId,
                SegaId,
                "Mega Drive / Genesis",
                1988,
                4,
                30.75m,
                true),
            new Console(
                DreamcastId,
                SegaId,
                "Dreamcast",
                1998,
                6,
                9.13m,
                true),
            new Console(
                PlayStation1Id,
                SonyId,
                "PlayStation",
                1994,
                5,
                102.49m,
                true),
            new Console(
                PlayStation2Id,
                SonyId,
                "PlayStation 2",
                2000,
                6,
                155.00m,
                true),
            new Console(
                Atari2600Id,
                AtariId,
                "Atari 2600",
                1977,
                2,
                30.00m,
                true),
            new Console(
                XboxId,
                MicrosoftId,
                "Xbox",
                2001,
                6,
                24.00m,
                true)
        );
    }
}
