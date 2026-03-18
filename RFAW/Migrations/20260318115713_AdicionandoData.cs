using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RFAW.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataSorteio",
                table: "Rifas",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataSorteio",
                table: "Rifas");
        }
    }
}
