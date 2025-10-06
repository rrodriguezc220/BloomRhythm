using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace HackathonBloomWatch.Migrations
{
    /// <inheritdoc />
    public partial class InitialWithDecimalFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campania",
                columns: table => new
                {
                    IdCampania = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCampania = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AnioCampania = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    FechaProceso = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campania", x => x.IdCampania);
                });

            migrationBuilder.CreateTable(
                name: "EspeciePlanta",
                columns: table => new
                {
                    IdEspeciePlanta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreEspecie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NombreComun = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImagenEspecie = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspeciePlanta", x => x.IdEspeciePlanta);
                });

            migrationBuilder.CreateTable(
                name: "MacizoForestal",
                columns: table => new
                {
                    IdMacizoForestal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Departamento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Provincia = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: true),
                    Distrito = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: true),
                    Localidad = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: true),
                    AreaHectareas = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    CoordenadaEste = table.Column<decimal>(type: "decimal(12,6)", nullable: true),
                    CoordenadaNorte = table.Column<decimal>(type: "decimal(12,6)", nullable: true),
                    Geometria = table.Column<Geometry>(type: "geography", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MacizoForestal", x => x.IdMacizoForestal);
                });

            migrationBuilder.CreateTable(
                name: "CampaniaDetalle",
                columns: table => new
                {
                    IdCampaniaDetalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCampania = table.Column<int>(type: "int", nullable: false),
                    TipoActividad = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    EstadoActividad = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    FechaActividad = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdMacizoForestal = table.Column<int>(type: "int", nullable: false),
                    IdEspeciePlanta = table.Column<int>(type: "int", nullable: false),
                    CantidadElementos = table.Column<int>(type: "int", nullable: true),
                    MortandadPlantas = table.Column<int>(type: "int", nullable: true),
                    ValorMacizoForestal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Agroforestal = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaniaDetalle", x => x.IdCampaniaDetalle);
                    table.ForeignKey(
                        name: "FK_CampaniaDetalle_Campania_IdCampania",
                        column: x => x.IdCampania,
                        principalTable: "Campania",
                        principalColumn: "IdCampania",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaniaDetalle_EspeciePlanta_IdEspeciePlanta",
                        column: x => x.IdEspeciePlanta,
                        principalTable: "EspeciePlanta",
                        principalColumn: "IdEspeciePlanta",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaniaDetalle_MacizoForestal_IdMacizoForestal",
                        column: x => x.IdMacizoForestal,
                        principalTable: "MacizoForestal",
                        principalColumn: "IdMacizoForestal",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaniaDetalle_IdCampania",
                table: "CampaniaDetalle",
                column: "IdCampania");

            migrationBuilder.CreateIndex(
                name: "IX_CampaniaDetalle_IdEspeciePlanta",
                table: "CampaniaDetalle",
                column: "IdEspeciePlanta");

            migrationBuilder.CreateIndex(
                name: "IX_CampaniaDetalle_IdMacizoForestal",
                table: "CampaniaDetalle",
                column: "IdMacizoForestal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaniaDetalle");

            migrationBuilder.DropTable(
                name: "Campania");

            migrationBuilder.DropTable(
                name: "EspeciePlanta");

            migrationBuilder.DropTable(
                name: "MacizoForestal");
        }
    }
}
