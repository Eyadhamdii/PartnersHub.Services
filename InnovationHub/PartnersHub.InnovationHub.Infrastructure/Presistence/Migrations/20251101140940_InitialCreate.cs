using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartnersHub.InnovationHub.Infrastructure.Presistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssociatedProviders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociatedProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssociatedSectors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociatedSectors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChallengeRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SourceCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssociatedSectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmitterName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PriorityLevelId = table.Column<int>(type: "int", nullable: false),
                    ChallengeStatus = table.Column<int>(type: "int", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: true),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChallengeRequests_AssociatedProviders_SourceCompanyId",
                        column: x => x.SourceCompanyId,
                        principalTable: "AssociatedProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChallengeRequests_AssociatedSectors_AssociatedSectorId",
                        column: x => x.AssociatedSectorId,
                        principalTable: "AssociatedSectors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChallengeRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Format = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    Extension = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    Metadata_Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SharePointFileId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SharePointUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SharePointLibrary = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChallengeRequestId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_ChallengeRequests_ChallengeRequestId",
                        column: x => x.ChallengeRequestId,
                        principalTable: "ChallengeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachments_ChallengeRequests_ChallengeRequestId1",
                        column: x => x.ChallengeRequestId1,
                        principalTable: "ChallengeRequests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "challengeRequestRevisionComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChallengeRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_challengeRequestRevisionComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_challengeRequestRevisionComments_ChallengeRequests_ChallengeRequestId",
                        column: x => x.ChallengeRequestId,
                        principalTable: "ChallengeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "challengeTrackingHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChallengeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_challengeTrackingHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_challengeTrackingHistories_ChallengeRequests_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "ChallengeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "technologies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechnologyStage = table.Column<int>(type: "int", nullable: false),
                    Sector = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChallengeRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technologies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_technologies_ChallengeRequests_ChallengeRequestId",
                        column: x => x.ChallengeRequestId,
                        principalTable: "ChallengeRequests",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "challengeTechnologiesRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChallengeRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TechnologyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JustificationForLinking = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RequestStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_challengeTechnologiesRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_challengeTechnologiesRequests_technologies_TechnologyId",
                        column: x => x.TechnologyId,
                        principalTable: "technologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ChallengeRequestId",
                table: "Attachments",
                column: "ChallengeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ChallengeRequestId1",
                table: "Attachments",
                column: "ChallengeRequestId1");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_IsDeleted",
                table: "Attachments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_UploadedAt",
                table: "Attachments",
                column: "UploadedAt");

            migrationBuilder.CreateIndex(
                name: "IX_challengeRequestRevisionComments_ChallengeRequestId",
                table: "challengeRequestRevisionComments",
                column: "ChallengeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeRequests_AssociatedSectorId",
                table: "ChallengeRequests",
                column: "AssociatedSectorId");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeRequests_SourceCompanyId",
                table: "ChallengeRequests",
                column: "SourceCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_challengeTechnologiesRequests_ChallengeRequestId_TechnologyId",
                table: "challengeTechnologiesRequests",
                columns: new[] { "ChallengeRequestId", "TechnologyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_challengeTechnologiesRequests_TechnologyId",
                table: "challengeTechnologiesRequests",
                column: "TechnologyId");

            migrationBuilder.CreateIndex(
                name: "IX_challengeTrackingHistories_ChallengeId",
                table: "challengeTrackingHistories",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_technologies_ChallengeRequestId",
                table: "technologies",
                column: "ChallengeRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "challengeRequestRevisionComments");

            migrationBuilder.DropTable(
                name: "challengeTechnologiesRequests");

            migrationBuilder.DropTable(
                name: "challengeTrackingHistories");

            migrationBuilder.DropTable(
                name: "technologies");

            migrationBuilder.DropTable(
                name: "ChallengeRequests");

            migrationBuilder.DropTable(
                name: "AssociatedProviders");

            migrationBuilder.DropTable(
                name: "AssociatedSectors");
        }
    }
}
