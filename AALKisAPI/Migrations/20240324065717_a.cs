using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AALKisAPI.Migrations
{
    /// <inheritdoc />
    public partial class a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_id",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "keywords");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "tags",
                type: "int(11)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "keywords",
                type: "int(11)",
                nullable: true);
        }
    }
}
