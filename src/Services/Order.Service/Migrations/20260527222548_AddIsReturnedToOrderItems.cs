using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddIsReturnedToOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Isreturned",
                table: "OrderItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Isreturned",
                table: "OrderItems");
        }
    }
}
