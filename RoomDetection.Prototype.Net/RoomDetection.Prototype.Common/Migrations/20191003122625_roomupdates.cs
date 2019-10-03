using Microsoft.EntityFrameworkCore.Migrations;

namespace RoomDetection.Prototype.Common.Migrations
{
    public partial class roomupdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomType",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "Rooms",
                newName: "TimeStampSince");

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "Rooms",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomStatus",
                table: "Rooms",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomStatus",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "TimeStampSince",
                table: "Rooms",
                newName: "LastUpdated");

            migrationBuilder.AddColumn<int>(
                name: "RoomType",
                table: "Rooms",
                nullable: false,
                defaultValue: 0);
        }
    }
}
