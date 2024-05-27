using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senium.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPropsCurriculoRelacionamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AreaDeAtuacao",
                table: "Curriculos",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Cep",
                table: "Curriculos",
                type: "longtext",
                nullable: false,
                defaultValue: "9")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Curriculos",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Clt",
                table: "Curriculos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EBaixaRenda",
                table: "Curriculos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EDeficienciaAuditiva",
                table: "Curriculos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EDeficienciaFisica",
                table: "Curriculos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EDeficienciaIntelectual",
                table: "Curriculos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EDeficienciaMotora",
                table: "Curriculos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EDeficienciaVisual",
                table: "Curriculos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ELgbtqia",
                table: "Curriculos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EPessoaComDeficiencia",
                table: "Curriculos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Curriculos",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Curriculos",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EstadoCivil",
                table: "Curriculos",
                type: "varchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Genero",
                table: "Curriculos",
                type: "varchar(9)",
                maxLength: 9,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "GrauDeFormacao",
                table: "Curriculos",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Hibrido",
                table: "Curriculos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Linkedin",
                table: "Curriculos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Pj",
                table: "Curriculos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Portfolio",
                table: "Curriculos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Presencial",
                table: "Curriculos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RacaEtnia",
                table: "Curriculos",
                type: "varchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Remoto",
                table: "Curriculos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ResumoProfissional",
                table: "Curriculos",
                type: "varchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "Curriculos",
                type: "varchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Temporario",
                table: "Curriculos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Titulo",
                table: "Curriculos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Curriculos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Curriculos_UsuarioId",
                table: "Curriculos",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Curriculos_Usuarios_UsuarioId",
                table: "Curriculos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

           /* migrationBuilder.AddForeignKey(
                name: "FK_Experiencias_Curriculos_CurriculoId",
                table: "Experiencias",
                column: "CurriculoId",
                principalTable: "Curriculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade); */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Curriculos_Usuarios_UsuarioId",
                table: "Curriculos");

            migrationBuilder.DropForeignKey(
                name: "FK_Experiencias_Curriculos_CurriculoId",
                table: "Experiencias");

            migrationBuilder.DropIndex(
                name: "IX_Experiencias_CurriculoId",
                table: "Experiencias");

            migrationBuilder.DropIndex(
                name: "IX_Curriculos_UsuarioId",
                table: "Curriculos");

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

            migrationBuilder.DropColumn(
                name: "AreaDeAtuacao",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "Cep",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "Clt",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "EBaixaRenda",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "EDeficienciaAuditiva",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "EDeficienciaFisica",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "EDeficienciaIntelectual",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "EDeficienciaMotora",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "EDeficienciaVisual",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "ELgbtqia",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "EPessoaComDeficiencia",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "EstadoCivil",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "Genero",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "GrauDeFormacao",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "Hibrido",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "Linkedin",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "Pj",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "Portfolio",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "Presencial",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "RacaEtnia",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "Remoto",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "ResumoProfissional",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "Temporario",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Curriculos");
        }
    }
}
