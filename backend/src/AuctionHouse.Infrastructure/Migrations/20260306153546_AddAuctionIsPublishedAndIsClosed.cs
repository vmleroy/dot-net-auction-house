using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuctionIsPublishedAndIsClosed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "Auctions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Auctions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Auctions");
        }
    }
}
