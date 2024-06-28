using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senium.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterandoRelacionamentoExp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiencias_Curriculos_CurriculoId",
                table: "Experiencias");

            migrationBuilder.RenameColumn(
                name: "CurriculoId",
                table: "Experiencias",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Experiencias_CurriculoId",
                table: "Experiencias",
                newName: "IX_Experiencias_UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiencias_Usuarios_UsuarioId",
                table: "Experiencias",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiencias_Usuarios_UsuarioId",
                table: "Experiencias");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Experiencias",
                newName: "CurriculoId");

            migrationBuilder.RenameIndex(
                name: "IX_Experiencias_UsuarioId",
                table: "Experiencias",
                newName: "IX_Experiencias_CurriculoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiencias_Curriculos_CurriculoId",
                table: "Experiencias",
                column: "CurriculoId",
                principalTable: "Curriculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
