using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskBoard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedOtherRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
            { Guid.Parse("10000000-0000-0000-0000-000000000001"), "Admin" },
            { Guid.Parse("10000000-0000-0000-0000-000000000002"), "Manager" },
            { Guid.Parse("10000000-0000-0000-0000-000000000003"), "Developer" }
                });
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
