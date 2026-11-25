using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AspNetCoreSecuritySection3.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductName = table.Column<string>(type: "TEXT", nullable: false),
                    ReviewerName = table.Column<string>(type: "TEXT", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewText = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    AttachmentPath = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SurveySubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    SubmittedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveySubmissions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ProductReviews",
                columns: new[] { "Id", "CreatedAtUtc", "ProductName", "Rating", "ReviewText", "ReviewerName" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 10, 11, 0, 0, 0, DateTimeKind.Utc), "Wireless Keyboard", 4, "Solid build quality and comfortable to type on.", "Eve Turner" },
                    { 2, new DateTime(2026, 2, 20, 16, 0, 0, 0, DateTimeKind.Utc), "USB-C Hub", 2, "Stopped working after a week.", "Frank Miller" }
                });

            migrationBuilder.InsertData(
                table: "SupportTickets",
                columns: new[] { "Id", "AttachmentPath", "Category", "CreatedAtUtc", "Description", "Email", "FullName", "Title" },
                values: new object[,]
                {
                    { 1, null, "Billing", new DateTime(2026, 1, 20, 10, 0, 0, 0, DateTimeKind.Utc), "My last invoice has an incorrect charge.", "carol@example.com", "Carol White", "Invoice discrepancy" },
                    { 2, null, "Technical", new DateTime(2026, 2, 1, 8, 0, 0, 0, DateTimeKind.Utc), "Cannot log in after the recent update.", "david@example.com", "David Brown", "Login page error" }
                });

            migrationBuilder.InsertData(
                table: "SurveySubmissions",
                columns: new[] { "Id", "Comment", "Email", "Name", "Score", "SubmittedAtUtc" },
                values: new object[,]
                {
                    { 1, "Great service overall!", "alice@example.com", "Alice Johnson", 5, new DateTime(2026, 1, 10, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "Average experience.", "bob@example.com", "Bob Smith", 3, new DateTime(2026, 1, 15, 14, 30, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductReviews");

            migrationBuilder.DropTable(
                name: "SupportTickets");

            migrationBuilder.DropTable(
                name: "SurveySubmissions");
        }
    }
}
