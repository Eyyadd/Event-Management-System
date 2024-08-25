using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class updatehistorytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FeedbackId",
                table: "EventBooking",
                newName: "TotalNumberOfTickets");

            migrationBuilder.AlterColumn<long>(
                name: "TotalPrice",
                table: "EventBooking",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalNumberOfTickets",
                table: "EventBooking",
                newName: "FeedbackId");

            migrationBuilder.AlterColumn<int>(
                name: "TotalPrice",
                table: "EventBooking",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
