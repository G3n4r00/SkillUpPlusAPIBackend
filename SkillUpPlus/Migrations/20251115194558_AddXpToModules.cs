using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillUpPlus.Migrations
{
    /// <inheritdoc />
    public partial class AddXpToModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "XpPoints",
                table: "Modules",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XpPoints",
                table: "Modules");
        }
    }
}
