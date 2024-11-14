using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FazendaUniao.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlantacaoInsumoRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plantacao_Insumo_InsumoId",
                table: "Plantacao");

            migrationBuilder.AddForeignKey(
                name: "FK_Plantacao_Insumo_InsumoId",
                table: "Plantacao",
                column: "InsumoId",
                principalTable: "Insumo",
                principalColumn: "InsumoId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plantacao_Insumo_InsumoId",
                table: "Plantacao");

            migrationBuilder.AddForeignKey(
                name: "FK_Plantacao_Insumo_InsumoId",
                table: "Plantacao",
                column: "InsumoId",
                principalTable: "Insumo",
                principalColumn: "InsumoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
