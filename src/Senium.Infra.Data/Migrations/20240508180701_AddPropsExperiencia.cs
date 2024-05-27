using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senium.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPropsExperiencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cargo",
                table: "Experiencias",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "CurriculoId",
                table: "Experiencias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataDeInicio",
                table: "Experiencias",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataDeTermino",
                table: "Experiencias",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Experiencias",
                type: "varchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Empresa",
                table: "Experiencias",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "TrabalhoAtual",
                table: "Experiencias",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Experiencias_CurriculoId",
                table: "Experiencias",
                column: "CurriculoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiencias_Curriculos_CurriculoId",
                table: "Experiencias",
                column: "CurriculoId",
                principalTable: "Curriculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiencias_Curriculos_CurriculoId",
                table: "Experiencias");

            migrationBuilder.DropIndex(
                name: "IX_Experiencias_CurriculoId",
                table: "Experiencias");

            migrationBuilder.DropColumn(
                name: "Cargo",
                table: "Experiencias");

            migrationBuilder.DropColumn(
                name: "CurriculoId",
                table: "Experiencias");

            migrationBuilder.DropColumn(
                name: "DataDeInicio",
                table: "Experiencias");

            migrationBuilder.DropColumn(
                name: "DataDeTermino",
                table: "Experiencias");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Experiencias");

            migrationBuilder.DropColumn(
                name: "Empresa",
                table: "Experiencias");

            migrationBuilder.DropColumn(
                name: "TrabalhoAtual",
                table: "Experiencias");
        }
    }
}
