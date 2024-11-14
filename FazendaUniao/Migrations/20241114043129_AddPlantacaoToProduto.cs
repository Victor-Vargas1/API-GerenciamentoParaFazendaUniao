using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FazendaUniao.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantacaoToProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlantacaoId",
                table: "Produtos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_PlantacaoId",
                table: "Produtos",
                column: "PlantacaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Plantacao_PlantacaoId",
                table: "Produtos",
                column: "PlantacaoId",
                principalTable: "Plantacao",
                principalColumn: "PlantacaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Plantacao_PlantacaoId",
                table: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_PlantacaoId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "PlantacaoId",
                table: "Produtos");
        }
    }
}
