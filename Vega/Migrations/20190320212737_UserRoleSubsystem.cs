using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace vega.Migrations
{
    public partial class UserRoleSubsystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(maxLength: 255, nullable: false),
                    LastName = table.Column<string>(maxLength: 255, nullable: false),
                    Email = table.Column<string>(maxLength: 255, nullable: false),
                    MobilePhone = table.Column<string>(nullable: true),
                    EncryptedPassword = table.Column<string>(nullable: false),
                    Role = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "EncryptedPassword", "FirstName", "LastName", "MobilePhone", "Role" },
                values: new object[] { 1, "mihail.udot@gmail.com", "lZQNtAeGjog2ZZ42idg8QXd66HnrvZ3up36OTZ1wtttfXtI3LlGgxWZlP5rrco5oUcQ1/sl3U766KwbizCtY5BysPi2PGDR5yYUHuaJTtdaS2bCJEevjmuGxhYbBzEMBuckVpgKELnqNXx63zAsK0HkIo27ZD6vPVdv8CazS5JY=", "Mihail", "Udot", "+79222222222", 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
