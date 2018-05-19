using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace vega.Migrations {
    public partial class Seeddatabase : Migration{
        protected override void Up(MigrationBuilder migrationBuilder) { 
            migrationBuilder.Sql("INSERT INTO Makes (Name) VALUES ('Audi')");
            migrationBuilder.Sql("INSERT INTO Makes (Name) VALUES ('Volvo')");
            migrationBuilder.Sql("INSERT INTO Makes (Name) VALUES ('Toyota')");

            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) VALUES ('Q3', (SELECT ID FROM Makes WHERE Name = 'Audi' ))");
            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) VALUES ('Q5', (SELECT ID FROM Makes WHERE Name = 'Audi' ))");
            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) VALUES ('A7', (SELECT ID FROM Makes WHERE Name = 'Audi' ))");

            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) VALUES ('S80', (SELECT ID FROM Makes WHERE Name = 'Volvo' ))");
            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) VALUES ('XC60', (SELECT ID FROM Makes WHERE Name = 'Volvo' ))");
            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) VALUES ('XC90', (SELECT ID FROM Makes WHERE Name = 'Volvo' ))");
        
            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) VALUES ('Corolla', (SELECT ID FROM Makes WHERE Name = 'Toyota' ))");
            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) VALUES ('Camry', (SELECT ID FROM Makes WHERE Name = 'Toyota' ))");
            migrationBuilder.Sql("INSERT INTO Models (Name, MakeId) VALUES ('Land Cruiser 200', (SELECT ID FROM Makes WHERE Name = 'Toyota' ))");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
                migrationBuilder.Sql("DELETE FROM Models");
                migrationBuilder.Sql("DELETE FROM Makes");
        }
    }
}
