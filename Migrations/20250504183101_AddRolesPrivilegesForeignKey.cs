using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace studentsapi.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesPrivilegesForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RolesPrivileges_RoleId",
                table: "RolesPrivileges",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolesPrivileges_Roles_RoleId",
                table: "RolesPrivileges",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolesPrivileges_Roles_RoleId",
                table: "RolesPrivileges");

            migrationBuilder.DropIndex(
                name: "IX_RolesPrivileges_RoleId",
                table: "RolesPrivileges");
        }
    }
}
