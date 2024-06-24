using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senium.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdmDefault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var senha =
                "$argon2id$v=19$m=32768,t=4,p=1$t6YzFMR4/HFEQGZFxaddqQ$Dpi52QxuxLkdYthafcXYThvHhhx730RadwIIAbZuGzI";
            
            migrationBuilder.InsertData(
                table: "Administradores",
                columns: new[] { "Id", "CriadoEm", "AtualizadoEm", "Email", "Nome", "Senha",  },
                values: new object[,]
                {
                    { 1, "2024-06-12 15:40:48", "2024-06-12 15:40:48", "terezafurtado77@gmail.com", "Tereza Furtado", senha }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .DeleteData("Administradores", "Id", 1);
        }

    }
}
