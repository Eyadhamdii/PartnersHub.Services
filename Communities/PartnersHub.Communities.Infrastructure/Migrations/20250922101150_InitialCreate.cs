using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartnersHub.Communities.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Communities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommunityFollowers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FollowedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CommunityId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityFollowers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityFollowers_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityFollowers_Communities_CommunityId1",
                        column: x => x.CommunityId1,
                        principalTable: "Communities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommunityPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CommunityId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityPosts_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityPosts_Communities_CommunityId1",
                        column: x => x.CommunityId1,
                        principalTable: "Communities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommunityFollowers_CommunityId_UserId",
                table: "CommunityFollowers",
                columns: new[] { "CommunityId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommunityFollowers_CommunityId1",
                table: "CommunityFollowers",
                column: "CommunityId1");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityPosts_CommunityId",
                table: "CommunityPosts",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityPosts_CommunityId1",
                table: "CommunityPosts",
                column: "CommunityId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunityFollowers");

            migrationBuilder.DropTable(
                name: "CommunityPosts");

            migrationBuilder.DropTable(
                name: "Communities");
        }
    }
}
