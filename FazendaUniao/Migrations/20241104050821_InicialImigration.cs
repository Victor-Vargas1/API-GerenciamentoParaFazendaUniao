using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FazendaUniao.Migrations
{
    /// <inheritdoc />
    public partial class InicialImigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeCliente = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CnpjCliente = table.Column<string>(type: "TEXT", maxLength: 18, nullable: true),
                    EnderecoCliente = table.Column<string>(type: "TEXT", nullable: true),
                    TelefoneCliente = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    EmailCliente = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.ClienteId);
                });

            migrationBuilder.CreateTable(
                name: "Fornecedores",
                columns: table => new
                {
                    FornecedorId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 90, nullable: false),
                    Cnpj = table.Column<string>(type: "TEXT", maxLength: 18, nullable: false),
                    Endereco = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornecedores", x => x.FornecedorId);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    ProdutoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Categoria = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    PrecoUnitario = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.ProdutoId);
                });

            migrationBuilder.CreateTable(
                name: "Insumo",
                columns: table => new
                {
                    InsumoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FornecedorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insumo", x => x.InsumoId);
                    table.ForeignKey(
                        name: "FK_Insumo_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "FornecedorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstoqueProdutos",
                columns: table => new
                {
                    EstoqueProdutosId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProdutoId = table.Column<int>(type: "INTEGER", nullable: true),
                    QuantidadeEmEstoque = table.Column<int>(type: "INTEGER", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstoqueProdutos", x => x.EstoqueProdutosId);
                    table.ForeignKey(
                        name: "FK_EstoqueProdutos_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "ProdutoId");
                });

            migrationBuilder.CreateTable(
                name: "EstoqueInsumos",
                columns: table => new
                {
                    EstoqueInsumosId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InsumoId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantidadeEmEstoque = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstoqueInsumos", x => x.EstoqueInsumosId);
                    table.ForeignKey(
                        name: "FK_EstoqueInsumos_Insumo_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumo",
                        principalColumn: "InsumoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Plantacao",
                columns: table => new
                {
                    PlantacaoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InsumoId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoPlanta = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DataPlantio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataColheita = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    QuantidadePlantio = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plantacao", x => x.PlantacaoId);
                    table.ForeignKey(
                        name: "FK_Plantacao_Insumo_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumo",
                        principalColumn: "InsumoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    PedidoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProdutoId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    EstoqueProdutosId = table.Column<int>(type: "INTEGER", nullable: true),
                    DataPedido = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValorTotal = table.Column<double>(type: "REAL", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.PedidoId);
                    table.ForeignKey(
                        name: "FK_Pedido_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Cliente",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pedido_EstoqueProdutos_EstoqueProdutosId",
                        column: x => x.EstoqueProdutosId,
                        principalTable: "EstoqueProdutos",
                        principalColumn: "EstoqueProdutosId");
                    table.ForeignKey(
                        name: "FK_Pedido_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "ProdutoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstoqueInsumos_InsumoId",
                table: "EstoqueInsumos",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstoqueProdutos_ProdutoId",
                table: "EstoqueProdutos",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Insumo_FornecedorId",
                table: "Insumo",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_ClienteId",
                table: "Pedido",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_EstoqueProdutosId",
                table: "Pedido",
                column: "EstoqueProdutosId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_ProdutoId",
                table: "Pedido",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Plantacao_InsumoId",
                table: "Plantacao",
                column: "InsumoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstoqueInsumos");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Plantacao");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "EstoqueProdutos");

            migrationBuilder.DropTable(
                name: "Insumo");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Fornecedores");
        }
    }
}
