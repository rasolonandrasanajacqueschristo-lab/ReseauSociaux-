using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReseauUniversitaire.Migrations
{
    /// <inheritdoc />
    public partial class AjoutCanauxGroupes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Canaux",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    EstAdminSeulement = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GroupeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canaux", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Canaux_Groupes_GroupeId",
                        column: x => x.GroupeId,
                        principalTable: "Groupes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessagesCanaux",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Contenu = table.Column<string>(type: "text", nullable: false),
                    FichierUrl = table.Column<string>(type: "text", nullable: true),
                    DateEnvoi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CanalId = table.Column<int>(type: "integer", nullable: false),
                    ExpediteurId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagesCanaux", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessagesCanaux_Canaux_CanalId",
                        column: x => x.CanalId,
                        principalTable: "Canaux",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessagesCanaux_Utilisateurs_ExpediteurId",
                        column: x => x.ExpediteurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Canaux_GroupeId",
                table: "Canaux",
                column: "GroupeId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesCanaux_CanalId",
                table: "MessagesCanaux",
                column: "CanalId");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesCanaux_ExpediteurId",
                table: "MessagesCanaux",
                column: "ExpediteurId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessagesCanaux");

            migrationBuilder.DropTable(
                name: "Canaux");
        }
    }
}
