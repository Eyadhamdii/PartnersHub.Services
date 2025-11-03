using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartnersHub.Synergy.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSuccessStoryType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SuccessStories_SuccessStoryTypeId",
                table: "SuccessStories",
                column: "SuccessStoryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SuccessStories_SuccessStoryTypes_SuccessStoryTypeId",
                table: "SuccessStories",
                column: "SuccessStoryTypeId",
                principalTable: "SuccessStoryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SuccessStories_SuccessStoryTypes_SuccessStoryTypeId",
                table: "SuccessStories");

            migrationBuilder.DropIndex(
                name: "IX_SuccessStories_SuccessStoryTypeId",
                table: "SuccessStories");
        }
    }
}
