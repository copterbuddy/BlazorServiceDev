using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "UserDetail",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("fadaf2ef-f174-4344-ba3d-468fcc299a3c")),
                Email = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: true),
                Password = table.Column<string>(type: "text", nullable: false),
                Role = table.Column<string>(type: "text", nullable: true, defaultValue: "USER"),
                CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, defaultValue: new DateTimeOffset(new DateTime(2023, 4, 22, 15, 51, 37, 921, DateTimeKind.Unspecified).AddTicks(3287), new TimeSpan(0, 7, 0, 0, 0))),
                LastUpdated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, defaultValue: new DateTimeOffset(new DateTime(2023, 4, 22, 15, 51, 37, 921, DateTimeKind.Unspecified).AddTicks(3429), new TimeSpan(0, 7, 0, 0, 0)))
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserDetail", x => new { x.Id, x.Email });
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "UserDetail");
    }
}
