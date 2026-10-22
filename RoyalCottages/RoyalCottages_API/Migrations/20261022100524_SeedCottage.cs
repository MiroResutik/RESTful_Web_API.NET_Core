using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RoyalCottages_API.Migrations
{
    /// <inheritdoc />
    public partial class SeedCottage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cottage",
                columns: new[] { "Id", "CreatedDate", "Details", "ImageUrl", "Name", "Occupancy", "Rate", "SqMeters", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Experience a relaxing coastal retreat with breathtaking ocean views, spacious living areas, and exclusive access to a private beach.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg", "Royal Cottage", 6, 500.0, 250, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Enjoy an elegant mountain getaway featuring refined marble interiors, generous living spaces, and spectacular panoramic mountain views.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa2.jpg", "Diamond Cottage", 8, 750.0, 320, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Relax in a contemporary retreat with a stunning infinity pool, stylish interiors, and a private outdoor area perfect for entertaining.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg", "Pool Cottage", 4, 350.0, 180, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Indulge in an exceptional luxury escape with expansive interiors, premium amenities, spa facilities, and personalised concierge services.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa4.jpg", "Luxury Cottage", 10, 900.0, 4000, new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Unwind in a peaceful countryside hideaway surrounded by lush gardens, natural scenery, and scenic walking trails, ideal for a quiet escape.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa5.jpg", "Garden Cottage", 3, 275.0, 150, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cottage",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cottage",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cottage",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cottage",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Cottage",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
