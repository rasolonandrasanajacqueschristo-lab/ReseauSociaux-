using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReseauUniversitaire.Migrations
{
    /// <inheritdoc />
    public partial class AjoutMediasPublication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FichierNom",
                table: "Publications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FichierUrl",
                table: "Publications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "Publications",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FichierNom",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "FichierUrl",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "Publications");
        }
    }
}
