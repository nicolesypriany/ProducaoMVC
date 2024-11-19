using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Producao.Migrations
{
    /// <inheritdoc />
    public partial class InitialDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Maquinas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maquinas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MateriasPrimas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fornecedor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preco = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateriasPrimas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Medidas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PecasPorUnidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Formas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    PecasPorCiclo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Formas_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormaMaquina",
                columns: table => new
                {
                    FormasId = table.Column<int>(type: "int", nullable: false),
                    MaquinasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormaMaquina", x => new { x.FormasId, x.MaquinasId });
                    table.ForeignKey(
                        name: "FK_FormaMaquina_Formas_FormasId",
                        column: x => x.FormasId,
                        principalTable: "Formas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormaMaquina_Maquinas_MaquinasId",
                        column: x => x.MaquinasId,
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Producoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaquinaId = table.Column<int>(type: "int", nullable: false),
                    FormaId = table.Column<int>(type: "int", nullable: false),
                    Ciclos = table.Column<int>(type: "int", nullable: false),
                    QuantidadeProduzida = table.Column<double>(type: "float", nullable: false),
                    CustoUnitario = table.Column<double>(type: "float", nullable: false),
                    CustoTotal = table.Column<double>(type: "float", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Producoes_Formas_FormaId",
                        column: x => x.FormaId,
                        principalTable: "Formas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Producoes_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Producoes_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProducoesMateriasPrimas",
                columns: table => new
                {
                    ProducaoId = table.Column<int>(type: "int", nullable: false),
                    MateriaPrimaId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProducoesMateriasPrimas", x => new { x.ProducaoId, x.MateriaPrimaId });
                    table.ForeignKey(
                        name: "FK_ProducoesMateriasPrimas_MateriasPrimas_MateriaPrimaId",
                        column: x => x.MateriaPrimaId,
                        principalTable: "MateriasPrimas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProducoesMateriasPrimas_Producoes_ProducaoId",
                        column: x => x.ProducaoId,
                        principalTable: "Producoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormaMaquina_MaquinasId",
                table: "FormaMaquina",
                column: "MaquinasId");

            migrationBuilder.CreateIndex(
                name: "IX_Formas_ProdutoId",
                table: "Formas",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Producoes_FormaId",
                table: "Producoes",
                column: "FormaId");

            migrationBuilder.CreateIndex(
                name: "IX_Producoes_MaquinaId",
                table: "Producoes",
                column: "MaquinaId");

            migrationBuilder.CreateIndex(
                name: "IX_Producoes_ProdutoId",
                table: "Producoes",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProducoesMateriasPrimas_MateriaPrimaId",
                table: "ProducoesMateriasPrimas",
                column: "MateriaPrimaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormaMaquina");

            migrationBuilder.DropTable(
                name: "ProducoesMateriasPrimas");

            migrationBuilder.DropTable(
                name: "MateriasPrimas");

            migrationBuilder.DropTable(
                name: "Producoes");

            migrationBuilder.DropTable(
                name: "Formas");

            migrationBuilder.DropTable(
                name: "Maquinas");

            migrationBuilder.DropTable(
                name: "Produtos");
        }
    }
}
