using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace vega.Migrations
{
    public partial class SeedFeatures : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql("INSERT INTO Features (Name) VALUES ('Climate controle system')");  
            migrationBuilder.Sql("INSERT INTO Features (Name) VALUES ('x18 wheel disks') "); 
            migrationBuilder.Sql("INSERT INTO Features (Name) VALUES ('City-saver')");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql("DELETE FROM Features");
        }
    }
}
