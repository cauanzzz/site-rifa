using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RFAW.Migrations
{
    /// <inheritdoc />
    public partial class AddImagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Rifas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Imagem",
                table: "Rifas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Premio",
                table: "Rifas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Rifas");

            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Rifas");

            migrationBuilder.DropColumn(
                name: "Premio",
                table: "Rifas");
        }
    }
}
