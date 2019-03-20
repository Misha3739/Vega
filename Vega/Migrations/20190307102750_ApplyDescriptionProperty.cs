using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace vega.Migrations
{
    public partial class ApplyDescriptionProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Models",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Makes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Features",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Makes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Features");
        }
    }
}
