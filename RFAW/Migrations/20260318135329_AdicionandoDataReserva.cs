using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RFAW.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoDataReserva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataReserva",
                table: "Cotas",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataReserva",
                table: "Cotas");
        }
    }
}
