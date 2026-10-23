using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RoyalCottages_API.Migrations
{
    /// <inheritdoc />
    public partial class SeedCottage1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cottage",
                columns: new[] { "Id", "CreatedDate", "Details", "ImageUrl", "Name", "Occupancy", "Rate", "SqMeters", "UpdatedDate" },
                values: new object[,]
                {
                    { 6, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Wake up to refreshing sea breezes in this charming coastal retreat, featuring bright interiors, a private terrace, and beautiful seaside views.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg", "Seaside Cottage", 5, 425.0, 200, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Escape to a tranquil woodland hideaway surrounded by towering pine trees, rustic interiors, and peaceful outdoor spaces perfect for relaxing.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa2.jpg", "Pinewood Cottage", 4, 300.0, 160, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Enjoy unforgettable evenings in a stylish cottage with panoramic sunset views, comfortable living areas, and a spacious private balcony.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg", "Sunset Cottage", 5, 475.0, 190, new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Discover a peaceful lakeside retreat with scenic water views, cosy interiors, and a private outdoor seating area ideal for morning coffee.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa4.jpg", "Lakeside Cottage", 6, 550.0, 230, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Relax in a welcoming countryside cottage surrounded by open meadows, colourful wildflowers, and picturesque walking paths.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa5.jpg", "Meadow Cottage", 3, 250.0, 140, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Experience a memorable mountain escape with expansive windows, inviting living spaces, and breathtaking views of the surrounding peaks.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg", "Mountain View Cottage", 7, 625.0, 260, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Find comfort in this elegant cottage featuring warm wooden finishes, a beautifully landscaped garden, and peaceful spaces for the whole family.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa2.jpg", "Willow Cottage", 4, 325.0, 170, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 13, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Enjoy a contemporary getaway with sophisticated furnishings, an open-plan layout, and sweeping views across the surrounding countryside.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg", "Horizon Cottage", 8, 700.0, 300, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 14, new DateTime(2026, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Step into a charming retreat with tasteful interiors, a private garden filled with seasonal flowers, and inviting spaces for a relaxing stay.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa4.jpg", "Rosewood Cottage", 5, 375.0, 180, new DateTime(2026, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 15, new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Reconnect with nature in a spacious woodland retreat featuring natural materials, comfortable bedrooms, and a secluded patio surrounded by greenery.", "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa5.jpg", "Evergreen Cottage", 6, 450.0, 220, new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cottage",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Cottage",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Cottage",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Cottage",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Cottage",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Cottage",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Cottage",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Cottage",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Cottage",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Cottage",
                keyColumn: "Id",
                keyValue: 15);
        }
    }
}
