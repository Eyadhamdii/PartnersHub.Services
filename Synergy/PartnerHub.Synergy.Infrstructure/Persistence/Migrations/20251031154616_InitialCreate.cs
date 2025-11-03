using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PartnersHub.Synergy.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpportunityTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SuccessStories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    SuccessStoryTypeId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CollaborationStatusId = table.Column<int>(type: "int", nullable: false),
                    TermsAndConditionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TermsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    TermsAcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TermsAcceptedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuccessStories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SuccessStoryTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuccessStoryTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SynergyCompanies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HeadquarterCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HeadquarterCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RepresentativeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynergyCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThematicAreas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThematicAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SuccessStoryAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuccessStoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SharePointUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuccessStoryAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuccessStoryAttachments_SuccessStories_SuccessStoryId",
                        column: x => x.SuccessStoryId,
                        principalTable: "SuccessStories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SuccessStorySynergyCompanies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SynergyCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuccessStoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuccessStorySynergyCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuccessStorySynergyCompanies_SuccessStories_SuccessStoryId",
                        column: x => x.SuccessStoryId,
                        principalTable: "SuccessStories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SynergyCompanySectors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SynergyCompanySectors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SynergyCompanySectors_SynergyCompanies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "SynergyCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Opportunities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    OpportunityTypeId = table.Column<int>(type: "int", nullable: false),
                    ThematicAreaId = table.Column<int>(type: "int", nullable: false),
                    SectorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CollaborationRationale = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CollaborationRequirementOther = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ExpectedOutcomeOther = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TermsAndConditionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TermsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    TermsAcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TermsAcceptedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Opportunities_OpportunityTypes_OpportunityTypeId",
                        column: x => x.OpportunityTypeId,
                        principalTable: "OpportunityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Opportunities_ThematicAreas_ThematicAreaId",
                        column: x => x.ThematicAreaId,
                        principalTable: "ThematicAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollaborationRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpportunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollaborationRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollaborationRequirements_Opportunities_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExpectedOutcomes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpportunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpectedOutcomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpectedOutcomes_Opportunities_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OpportunityAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OpportunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SharePointUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpportunityAttachments_Opportunities_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpportunitySynergyCompanies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OpportunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SynergyCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CollaborationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunitySynergyCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpportunitySynergyCompanies_Opportunities_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SuccessStoryOpportunity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OpportunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuccessStoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuccessStoryOpportunity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuccessStoryOpportunity_Opportunities_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SuccessStoryOpportunity_SuccessStories_SuccessStoryId",
                        column: x => x.SuccessStoryId,
                        principalTable: "SuccessStories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CollaborationRequirements",
                columns: new[] { "Id", "Name", "OpportunityId" },
                values: new object[,]
                {
                    { 1, "Technology transfer", null },
                    { 2, "Joint R&D", null },
                    { 3, "Co-creation", null },
                    { 4, "Other", null }
                });

            migrationBuilder.InsertData(
                table: "ExpectedOutcomes",
                columns: new[] { "Id", "Name", "OpportunityId" },
                values: new object[,]
                {
                    { 1, "Revenue growth", null },
                    { 2, "Cost savings", null },
                    { 3, "Increased Efficiency", null },
                    { 4, "Other", null }
                });

            migrationBuilder.InsertData(
                table: "OpportunityTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Sponsorship" },
                    { 2, "Commercial Collaboration" },
                    { 3, "Strategic Collaboration" }
                });

            migrationBuilder.InsertData(
                table: "SuccessStoryTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Partnership" },
                    { 2, "Collaboration" },
                    { 3, "Joint Venture" }
                });

            migrationBuilder.InsertData(
                table: "ThematicAreas",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "ESG" },
                    { 2, "Digital Transformation" },
                    { 3, "Innovation" },
                    { 4, "Sustainability" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollaborationRequirements_OpportunityId",
                table: "CollaborationRequirements",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpectedOutcomes_OpportunityId",
                table: "ExpectedOutcomes",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_OpportunityTypeId",
                table: "Opportunities",
                column: "OpportunityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_ThematicAreaId",
                table: "Opportunities",
                column: "ThematicAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityAttachments_OpportunityId",
                table: "OpportunityAttachments",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunitySynergyCompanies_OpportunityId",
                table: "OpportunitySynergyCompanies",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunitySynergyCompanies_OpportunityId_SynergyCompanyId",
                table: "OpportunitySynergyCompanies",
                columns: new[] { "OpportunityId", "SynergyCompanyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpportunitySynergyCompanies_SynergyCompanyId",
                table: "OpportunitySynergyCompanies",
                column: "SynergyCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SuccessStoryAttachments_SuccessStoryId",
                table: "SuccessStoryAttachments",
                column: "SuccessStoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SuccessStoryOpportunity_OpportunityId",
                table: "SuccessStoryOpportunity",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_SuccessStoryOpportunity_SuccessStoryId",
                table: "SuccessStoryOpportunity",
                column: "SuccessStoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SuccessStorySynergyCompanies_SuccessStoryId",
                table: "SuccessStorySynergyCompanies",
                column: "SuccessStoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SuccessStorySynergyCompanies_SuccessStoryId_SynergyCompanyId",
                table: "SuccessStorySynergyCompanies",
                columns: new[] { "SuccessStoryId", "SynergyCompanyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SuccessStorySynergyCompanies_SynergyCompanyId",
                table: "SuccessStorySynergyCompanies",
                column: "SynergyCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SynergyCompanySectors_CompanyId",
                table: "SynergyCompanySectors",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SynergyCompanySectors_CompanyId_SectorId",
                table: "SynergyCompanySectors",
                columns: new[] { "CompanyId", "SectorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SynergyCompanySectors_SectorId",
                table: "SynergyCompanySectors",
                column: "SectorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollaborationRequirements");

            migrationBuilder.DropTable(
                name: "ExpectedOutcomes");

            migrationBuilder.DropTable(
                name: "OpportunityAttachments");

            migrationBuilder.DropTable(
                name: "OpportunitySynergyCompanies");

            migrationBuilder.DropTable(
                name: "SuccessStoryAttachments");

            migrationBuilder.DropTable(
                name: "SuccessStoryOpportunity");

            migrationBuilder.DropTable(
                name: "SuccessStorySynergyCompanies");

            migrationBuilder.DropTable(
                name: "SuccessStoryTypes");

            migrationBuilder.DropTable(
                name: "SynergyCompanySectors");

            migrationBuilder.DropTable(
                name: "Opportunities");

            migrationBuilder.DropTable(
                name: "SuccessStories");

            migrationBuilder.DropTable(
                name: "SynergyCompanies");

            migrationBuilder.DropTable(
                name: "OpportunityTypes");

            migrationBuilder.DropTable(
                name: "ThematicAreas");
        }
    }
}
