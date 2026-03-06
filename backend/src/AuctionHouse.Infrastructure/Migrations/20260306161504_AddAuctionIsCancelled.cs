using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuctionIsCancelled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "Auctions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "Auctions");
        }
    }
}
