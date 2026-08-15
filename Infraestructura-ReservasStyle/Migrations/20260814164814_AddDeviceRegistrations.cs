using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infraestructura_ReservasStyle.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceRegistrations",
                columns: table => new
                {
                    IdDeviceRegistration = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    FcmToken = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Plataforma = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceRegistrations", x => x.IdDeviceRegistration);
                    table.ForeignKey(
                        name: "FK_DeviceRegistrations_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceRegistrations_FcmToken",
                table: "DeviceRegistrations",
                column: "FcmToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceRegistrations_IdUsuario",
                table: "DeviceRegistrations",
                column: "IdUsuario");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceRegistrations");
        }
    }
}
