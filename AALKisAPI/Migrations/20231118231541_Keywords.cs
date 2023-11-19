using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AALKisAPI.Migrations
{
    /// <inheritdoc />
    public partial class Keywords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "keywords",
                columns: table => new
                {
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValueSql: "''")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    note_id = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.name, x.note_id });
                    table.ForeignKey(
                        name: "keywords_ibfk_1",
                        column: x => x.note_id,
                        principalTable: "notes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_keywords_note_id",
                table: "keywords",
                column: "note_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "keywords");
        }
    }
}
