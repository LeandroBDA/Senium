using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senium.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPropsAdm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Administradores",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Administradores",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Senha",
                table: "Administradores",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Administradores");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Administradores");

            migrationBuilder.DropColumn(
                name: "Senha",
                table: "Administradores");
        }
    }
}
