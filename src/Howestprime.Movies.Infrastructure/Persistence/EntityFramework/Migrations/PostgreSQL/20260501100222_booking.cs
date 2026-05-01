using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Migrations.PostgreSQL
{
    /// <inheritdoc />
    public partial class booking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bookings",
                table: "MovieEvents",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Visitors",
                table: "MovieEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bookings",
                table: "MovieEvents");

            migrationBuilder.DropColumn(
                name: "Visitors",
                table: "MovieEvents");
        }
    }
}
