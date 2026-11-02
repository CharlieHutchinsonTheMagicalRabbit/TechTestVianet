using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RetroGaming.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    HqCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HqCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FoundedYear = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Consoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManufacturerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ReleaseYear = table.Column<int>(type: "int", nullable: false),
                    Generation = table.Column<int>(type: "int", nullable: false),
                    UnitsSoldInMillions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsDiscontinued = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consoles_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Manufacturers",
                columns: new[] { "Id", "FoundedYear", "HqCity", "HqCountry", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 1889, "Kyoto", "Japan", 34.969900000000003, 135.75630000000001, "Nintendo" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 1960, "Tokyo", "Japan", 35.626399999999997, 139.74019999999999, "Sega" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), 1993, "San Mateo", "USA", 37.548499999999997, -122.31310000000001, "Sony Interactive Entertainment" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), 1972, "Sunnyvale", "USA", 37.3688, -122.0363, "Atari" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), 1975, "Redmond", "USA", 47.642299999999999, -122.1391, "Microsoft" }
                });

            migrationBuilder.InsertData(
                table: "Consoles",
                columns: new[] { "Id", "Generation", "IsDiscontinued", "ManufacturerId", "Name", "ReleaseYear", "UnitsSoldInMillions" },
                values: new object[,]
                {
                    { new Guid("a1111111-1111-1111-1111-111111111111"), 3, true, new Guid("11111111-1111-1111-1111-111111111111"), "NES / Famicom", 1983, 61.91m },
                    { new Guid("a2222222-2222-2222-2222-222222222222"), 4, true, new Guid("11111111-1111-1111-1111-111111111111"), "Super Nintendo", 1990, 49.10m },
                    { new Guid("a3333333-3333-3333-3333-333333333333"), 5, true, new Guid("11111111-1111-1111-1111-111111111111"), "N64", 1996, 32.93m },
                    { new Guid("b1111111-1111-1111-1111-111111111111"), 3, true, new Guid("22222222-2222-2222-2222-222222222222"), "Master System", 1985, 13.00m },
                    { new Guid("b2222222-2222-2222-2222-222222222222"), 4, true, new Guid("22222222-2222-2222-2222-222222222222"), "Mega Drive / Genesis", 1988, 30.75m },
                    { new Guid("b3333333-3333-3333-3333-333333333333"), 6, true, new Guid("22222222-2222-2222-2222-222222222222"), "Dreamcast", 1998, 9.13m },
                    { new Guid("c1111111-1111-1111-1111-111111111111"), 5, true, new Guid("33333333-3333-3333-3333-333333333333"), "PlayStation", 1994, 102.49m },
                    { new Guid("c2222222-2222-2222-2222-222222222222"), 6, true, new Guid("33333333-3333-3333-3333-333333333333"), "PlayStation 2", 2000, 155.00m },
                    { new Guid("d1111111-1111-1111-1111-111111111111"), 2, true, new Guid("44444444-4444-4444-4444-444444444444"), "Atari 2600", 1977, 30.00m },
                    { new Guid("e1111111-1111-1111-1111-111111111111"), 6, true, new Guid("55555555-5555-5555-5555-555555555555"), "Xbox", 2001, 24.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consoles_ManufacturerId_Name",
                table: "Consoles",
                columns: new[] { "ManufacturerId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturers_Name",
                table: "Manufacturers",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Consoles");

            migrationBuilder.DropTable(
                name: "Manufacturers");
        }
    }
}
