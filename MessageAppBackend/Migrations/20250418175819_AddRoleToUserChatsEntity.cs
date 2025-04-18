using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleToUserChatsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "UserChats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "UserChats");
        }
    }
}
