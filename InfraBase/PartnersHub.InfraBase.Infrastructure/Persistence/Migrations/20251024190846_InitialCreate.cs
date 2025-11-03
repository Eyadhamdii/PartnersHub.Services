using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartnersHub.InfraBase.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InfraRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProjectDescription = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    SectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubSectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetTypeOtherDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TenderingStage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FundingModel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartConstructionQuarter = table.Column<int>(type: "int", nullable: true),
                    StartConstructionYear = table.Column<int>(type: "int", nullable: true),
                    EndConstructionQuarter = table.Column<int>(type: "int", nullable: true),
                    EndConstructionYear = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubmittedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    RejectedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfraRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InfraRequestAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SharePointFileId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SharePointUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SharePointLibrary = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfraRequestAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InfraRequestAttachments_InfraRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "InfraRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InfraRequestHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    PerformedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FieldsChanged = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OldValues = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfraRequestHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InfraRequestHistories_InfraRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "InfraRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InfraRequestItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfraRequestItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InfraRequestItems_InfraRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "InfraRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InfraRequestItemFinancialDistributions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmountType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfraRequestItemFinancialDistributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InfraRequestItemFinancialDistributions_InfraRequestItems_RequestItemId",
                        column: x => x.RequestItemId,
                        principalTable: "InfraRequestItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InfraRequestAttachments_IsDeleted",
                table: "InfraRequestAttachments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InfraRequestAttachments_RequestId",
                table: "InfraRequestAttachments",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_InfraRequestAttachments_UploadedAt",
                table: "InfraRequestAttachments",
                column: "UploadedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InfraRequestHistories_RequestId",
                table: "InfraRequestHistories",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_InfraRequestItemFinancialDistributions_RequestItemId",
                table: "InfraRequestItemFinancialDistributions",
                column: "RequestItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InfraRequestItemFinancialDistributions_Year",
                table: "InfraRequestItemFinancialDistributions",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_InfraRequestItems_ItemCode",
                table: "InfraRequestItems",
                column: "ItemCode");

            migrationBuilder.CreateIndex(
                name: "IX_InfraRequestItems_RequestId",
                table: "InfraRequestItems",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InfraRequestAttachments");

            migrationBuilder.DropTable(
                name: "InfraRequestHistories");

            migrationBuilder.DropTable(
                name: "InfraRequestItemFinancialDistributions");

            migrationBuilder.DropTable(
                name: "InfraRequestItems");

            migrationBuilder.DropTable(
                name: "InfraRequests");
        }
    }
}
