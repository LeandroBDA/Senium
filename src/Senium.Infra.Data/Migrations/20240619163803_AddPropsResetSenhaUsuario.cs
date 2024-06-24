using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senium.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPropsResetSenhaUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiraResetToken",
                table: "Usuarios",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                    name: "TokenDeResetSenha",
                    table: "Usuarios",
                    type: "longtext",
                    nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiraResetToken",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TokenDeResetSenha",
                table: "Usuarios");
        }
    }
}
