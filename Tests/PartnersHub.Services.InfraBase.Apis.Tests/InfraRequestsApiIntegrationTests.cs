using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NUnit.Framework;
using PartnersHub.InfraBase.Apis;
using PartnersHub.InfraBase.Apis.Common;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;
using PartnersHub.InfraBase.Application.Common.Models;
using PartnersHub.InfraBase.Domain.Enums;
using PartnersHub.InfraBase.Infrastructure.Persistence;
using PartnersHub.InfraBase.Infrastructure.Persistence.Repositories;
using PartnersHub.InfraBase.Infrastructure.Services;

namespace PartnersHub.Services.InfraBase.Apis.Tests;

/// <summary>
/// Custom WebApplicationFactory for API integration tests
/// Configures in-memory database for isolated testing
/// </summary>
public class InfraBaseApiFactory : WebApplicationFactory<Program> {
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder) {
        builder.ConfigureServices(services => {
            // Remove existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<InfrabaseDbContext>));
            if (descriptor != null) {
                services.Remove(descriptor);
            }

            // Add in-memory database for testing
            services.AddDbContext<InfrabaseDbContext>(options => {
                options.UseInMemoryDatabase($"InfraBaseApiTests_{Guid.NewGuid()}");
            });

            // Ensure services are registered
            services.AddScoped<IInfrabaseRequestRepository, InfrabaseRequestRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<INotificationService, NotificationService>();

            // Build service provider and create database
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InfrabaseDbContext>();
            db.Database.EnsureCreated();
        });
    }
}

/// <summary>
/// Comprehensive API integration tests for InfraBase
/// Tests all API endpoints end-to-end
/// </summary>
[TestFixture]
public class InfraRequestsApiIntegrationTests {
    private InfraBaseApiFactory _factory = null!;
    private HttpClient _client = null!;
    private JsonSerializerOptions _jsonOptions = null!;

    [SetUp]
    public void SetUp() {
        _factory = new InfraBaseApiFactory();
        _client = _factory.CreateClient();
        
        // Configure JSON options for enum handling
        _jsonOptions = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    [TearDown]
    public void TearDown() {
        _client?.Dispose();
        _factory?.Dispose();
    }

    #region Complete End-to-End Workflow Test

    [Test]
    public async Task CompleteWorkflow_CreateSubmitApproveReject_Success() {
        var userId = Guid.NewGuid();
        var pcAdminId = Guid.NewGuid();
        var infraAdminId = Guid.NewGuid();

        // 1. CREATE REQUEST
        var createCommand = new {
            ProjectName = "E2E Test Project",
            ProjectDescription = "Complete workflow test",
            SectorId = Guid.NewGuid(),
            SubSectorId = Guid.NewGuid(),
            AssetTypeId = Guid.NewGuid(),
            AssetTypeOtherDescription = (string?)null,
            TenderingStage = TenderingStage.PreTender,
            FundingModel = FundingModel.FullyGovernmentFunded,
            CreatedBy = userId,
            CompanyId = (Guid?)null,
            CompanyName = "TestCo"
        };

        var createResp = await _client.PostAsJsonAsync("/api/InfraRequests", createCommand, _jsonOptions);
        Assert.That(createResp.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        
        var createBody = await createResp.Content.ReadFromJsonAsync<ApiResponse<Guid>>(_jsonOptions);
        var requestId = createBody!.Data;
        Assert.That(requestId, Is.Not.EqualTo(Guid.Empty));

        // 2. ADD ITEM
        var addItemCommand = new {
            RequestId = requestId,
            ItemCode = "ITEM-001",
            ItemName = "Test Item",
            UomId = Guid.NewGuid(),
            Quantity = 10m,
            UnitPrice = 1000m
        };

        var addItemResp = await _client.PostAsJsonAsync($"/api/InfraRequests/{requestId}/items", addItemCommand, _jsonOptions);
        Assert.That(addItemResp.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        
        var addItemBody = await addItemResp.Content.ReadFromJsonAsync<ApiResponse<Guid>>(_jsonOptions);
        var itemId = addItemBody!.Data;

        // 3. ADD FINANCIAL DISTRIBUTION
        var addDistCommand = new {
            RequestId = requestId,
            ItemId = itemId,
            AmountType = AmountType.CAPEX,
            Year = 1,
            Amount = 10000m
        };

        var addDistResp = await _client.PostAsJsonAsync(
            $"/api/InfraRequests/{requestId}/items/{itemId}/distributions", 
            addDistCommand, 
            _jsonOptions);
        Assert.That(addDistResp.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        // 4. ADD ATTACHMENT
        var addAttachCommand = new {
            RequestId = requestId,
            FileName = "test-document.pdf",
            FileSizeInBytes = 1024000L,
            ContentType = "application/pdf",
            SharePointFileId = "SP-FILE-123",
            SharePointUrl = "https://sharepoint.test.com/file.pdf",
            SharePointLibrary = "InfraBase",
            UploadedBy = userId
        };

        var addAttachResp = await _client.PostAsJsonAsync(
            $"/api/InfraRequests/{requestId}/attachments", 
            addAttachCommand, 
            _jsonOptions);
        Assert.That(addAttachResp.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        // 5. SUBMIT REQUEST
        var submitResp = await _client.PostAsync(
            $"/api/InfraRequests/{requestId}/submit?userId={userId}", 
            null);
        Assert.That(submitResp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var submitBody = await submitResp.Content.ReadFromJsonAsync<ApiResponse<string>>(_jsonOptions);
        var requestCode = submitBody!.Data;
        Assert.That(!string.IsNullOrEmpty(requestCode));

        // 6. VERIFY STATUS - PENDING PC ADMIN APPROVAL
        var getResp = await _client.GetAsync($"/api/InfraRequests/by-status/{RequestStatus.PendingPcAdminApproval}?pageNumber=1&pageSize=10");
        Assert.That(getResp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var getBody = await getResp.Content.ReadFromJsonAsync<ApiResponse<PaginatedList<object>>>(_jsonOptions);
        Assert.That(getBody!.Data.TotalCount, Is.GreaterThanOrEqualTo(1));

        // 7. PC ADMIN ACCEPTS
        var acceptResp = await _client.PostAsync(
            $"/api/InfraRequests/{requestId}/pc-admin-accept?userId={pcAdminId}", 
            null);
        Assert.That(acceptResp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // 8. VERIFY STATUS - PENDING INFRABASE APPROVAL
        var getResp2 = await _client.GetAsync($"/api/InfraRequests/by-status/{RequestStatus.PendingPcInfrabaseApproval}");
        Assert.That(getResp2.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // 9. INFRABASE ADMIN APPROVES
        var approveResp = await _client.PostAsync(
            $"/api/InfraRequests/{requestId}/infrabase-admin-accept?userId={infraAdminId}", 
            null);
        Assert.That(approveResp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // 10. VERIFY FINAL STATUS - APPROVED
        var getFinalResp = await _client.GetAsync($"/api/InfraRequests/by-status/{RequestStatus.InfrabaseApproved}");
        Assert.That(getFinalResp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    #endregion

    #region Request Creation Tests

    [Test]
    public async Task CreateRequest_ValidData_ReturnsCreated() {
        // Arrange
        var command = new {
            ProjectName = "API Test Project",
            ProjectDescription = "Testing API",
            SectorId = Guid.NewGuid(),
            SubSectorId = Guid.NewGuid(),
            AssetTypeId = Guid.NewGuid(),
            AssetTypeOtherDescription = (string?)null,
            TenderingStage = TenderingStage.PreTender,
            FundingModel = FundingModel.FullyGovernmentFunded,
            CreatedBy = Guid.NewGuid(),
            CompanyId = (Guid?)null,
            CompanyName = "Test Company"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/InfraRequests", command, _jsonOptions);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(_jsonOptions);
        Assert.That(body!.Success, Is.True);
        Assert.That(body.Data, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public async Task CreateRequest_InvalidData_ReturnsBadRequest() {
        // Arrange - empty project name
        var command = new {
            ProjectName = "",
            ProjectDescription = "Test",
            SectorId = Guid.NewGuid(),
            SubSectorId = Guid.NewGuid(),
            AssetTypeId = Guid.NewGuid(),
            TenderingStage = TenderingStage.PreTender,
            FundingModel = FundingModel.FullyGovernmentFunded,
            CreatedBy = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/InfraRequests", command, _jsonOptions);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    #endregion

    #region Query Tests

    [Test]
    public async Task GetAllRequests_ReturnsOk() {
        // Act
        var response = await _client.GetAsync("/api/InfraRequests?pageNumber=1&pageSize=10");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedList<object>>>(_jsonOptions);
        Assert.That(body!.Success, Is.True);
        Assert.That(body.Data, Is.Not.Null);
    }

    [Test]
    public async Task GetRequestsByStatus_ValidStatus_ReturnsFiltered() {
        // Arrange - Create and submit a request
        var requestId = await CreateTestRequest();
        await SubmitTestRequest(requestId);

        // Act
        var response = await _client.GetAsync($"/api/InfraRequests/by-status/{RequestStatus.PendingPcAdminApproval}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedList<object>>>(_jsonOptions);
        Assert.That(body!.Data.TotalCount, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public async Task GetRequestsByUser_ValidUserId_ReturnsUserRequests() {
        // Arrange
        var userId = Guid.NewGuid();
        await CreateTestRequestForUser(userId);
        await CreateTestRequestForUser(userId);

        // Act
        var response = await _client.GetAsync($"/api/InfraRequests/user/{userId}?pageNumber=1&pageSize=10");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedList<object>>>(_jsonOptions);
        Assert.That(body!.Data.TotalCount, Is.EqualTo(2));
    }

    #endregion

    #region Workflow Tests

    [Test]
    public async Task SubmitRequest_ValidRequest_ReturnsRequestCode() {
        // Arrange
        var requestId = await CreateCompleteTestRequest();
        var userId = Guid.NewGuid();

        // Act
        var response = await _client.PostAsync($"/api/InfraRequests/{requestId}/submit?userId={userId}", null);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<string>>(_jsonOptions);
        Assert.That(body!.Success, Is.True);
        Assert.That(body.Data, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task AcceptByPcAdmin_PendingRequest_ChangesStatus() {
        // Arrange
        var requestId = await CreateCompleteTestRequest();
        await SubmitTestRequest(requestId);
        var pcAdminId = Guid.NewGuid();

        // Act
        var response = await _client.PostAsync(
            $"/api/InfraRequests/{requestId}/pc-admin-accept?userId={pcAdminId}", 
            null);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(_jsonOptions);
        Assert.That(body!.Success, Is.True);
    }

    [Test]
    public async Task RejectByPcAdmin_WithReason_RejectsRequest() {
        // Arrange
        var requestId = await CreateCompleteTestRequest();
        await SubmitTestRequest(requestId);
        
        var rejectCommand = new {
            RequestId = requestId,
            UserId = Guid.NewGuid(),
            RejectionReason = "Insufficient documentation provided"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/InfraRequests/{requestId}/pc-admin-reject", 
            rejectCommand, 
            _jsonOptions);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(_jsonOptions);
        Assert.That(body!.Success, Is.True);
    }

    [Test]
    public async Task AcceptByInfrabaseAdmin_PendingRequest_ApprovesRequest() {
        // Arrange
        var requestId = await CreateCompleteTestRequest();
        await SubmitTestRequest(requestId);
        
        var pcAdminId = Guid.NewGuid();
        await _client.PostAsync($"/api/InfraRequests/{requestId}/pc-admin-accept?userId={pcAdminId}", null);
        
        var infraAdminId = Guid.NewGuid();

        // Act
        var response = await _client.PostAsync(
            $"/api/InfraRequests/{requestId}/infrabase-admin-accept?userId={infraAdminId}", 
            null);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task RequestChangesByInfrabaseAdmin_PendingRequest_RequestsChanges() {
        // Arrange
        var requestId = await CreateCompleteTestRequest();
        await SubmitTestRequest(requestId);
        await _client.PostAsync($"/api/InfraRequests/{requestId}/pc-admin-accept?userId={Guid.NewGuid()}", null);
        
        var changeCommand = new {
            RequestId = requestId,
            UserId = Guid.NewGuid(),
            ChangeRequestDescription = "Please provide more detailed cost breakdown"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/InfraRequests/{requestId}/infrabase-admin-request-changes", 
            changeCommand, 
            _jsonOptions);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    #endregion

    #region Item Management Tests

    [Test]
    public async Task AddRequestItem_ValidData_AddsItem() {
        // Arrange
        var requestId = await CreateTestRequest();
        var command = new {
            RequestId = requestId,
            ItemCode = "ITM-TEST-001",
            ItemName = "Test Equipment",
            UomId = Guid.NewGuid(),
            Quantity = 5m,
            UnitPrice = 5000m
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/InfraRequests/{requestId}/items", command, _jsonOptions);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(_jsonOptions);
        Assert.That(body!.Data, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public async Task UpdateRequestItem_ValidData_UpdatesItem() {
        // Arrange
        var requestId = await CreateTestRequest();
        var itemId = await AddTestItem(requestId);
        
        var updateCommand = new {
            RequestId = requestId,
            ItemId = itemId,
            Quantity = 10m,
            UnitPrice = 6000m
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/InfraRequests/{requestId}/items/{itemId}", 
            updateCommand, 
            _jsonOptions);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task RemoveRequestItem_ExistingItem_RemovesItem() {
        // Arrange
        var requestId = await CreateTestRequest();
        var itemId = await AddTestItem(requestId);

        // Act
        var response = await _client.DeleteAsync($"/api/InfraRequests/{requestId}/items/{itemId}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    #endregion

    #region Financial Distribution Tests

    [Test]
    public async Task AddFinancialDistribution_ValidData_AddsDistribution() {
        // Arrange
        var requestId = await CreateTestRequest();
        var itemId = await AddTestItem(requestId);
        
        var command = new {
            RequestId = requestId,
            ItemId = itemId,
            AmountType = AmountType.CAPEX,
            Year = 1,
            Amount = 25000m
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/InfraRequests/{requestId}/items/{itemId}/distributions", 
            command, 
            _jsonOptions);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task UpdateFinancialDistribution_ValidData_UpdatesAmount() {
        // Arrange
        var requestId = await CreateTestRequest();
        var itemId = await AddTestItem(requestId);
        var distId = await AddTestDistribution(requestId, itemId);
        
        var updateCommand = new {
            RequestId = requestId,
            ItemId = itemId,
            DistributionId = distId,
            Amount = 30000m
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/InfraRequests/{requestId}/items/{itemId}/distributions/{distId}", 
            updateCommand, 
            _jsonOptions);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task RemoveFinancialDistribution_ExistingDistribution_Removes() {
        // Arrange
        var requestId = await CreateTestRequest();
        var itemId = await AddTestItem(requestId);
        var distId = await AddTestDistribution(requestId, itemId);

        // Act
        var response = await _client.DeleteAsync(
            $"/api/InfraRequests/{requestId}/items/{itemId}/distributions/{distId}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    #endregion

    #region Attachment Tests

    [Test]
    public async Task AddAttachment_ValidData_AddsAttachment() {
        // Arrange
        var requestId = await CreateTestRequest();
        var command = new {
            RequestId = requestId,
            FileName = "project-plan.pdf",
            FileSizeInBytes = 2048000L,
            ContentType = "application/pdf",
            SharePointFileId = "SP-FILE-456",
            SharePointUrl = "https://sharepoint.test.com/plan.pdf",
            SharePointLibrary = "InfraBase",
            UploadedBy = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/InfraRequests/{requestId}/attachments", 
            command, 
            _jsonOptions);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task GetRequestAttachments_ExistingRequest_ReturnsAttachments() {
        // Arrange
        var requestId = await CreateTestRequest();
        await AddTestAttachment(requestId);

        // Act
        var response = await _client.GetAsync($"/api/InfraRequests/{requestId}/attachments");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<object>>>(_jsonOptions);
        Assert.That(body!.Data.Count, Is.GreaterThanOrEqualTo(1));
    }

    [Test]
    public async Task RemoveAttachment_ExistingAttachment_SoftDeletes() {
        // Arrange
        var requestId = await CreateTestRequest();
        var attachmentId = await AddTestAttachment(requestId);
        var userId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync(
            $"/api/InfraRequests/{requestId}/attachments/{attachmentId}?userId={userId}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    #endregion

    #region Update Tests

    [Test]
    public async Task UpdateRequestBasicInfo_DraftRequest_Updates() {
        // Arrange
        var requestId = await CreateTestRequest();
        var updateCommand = new {
            RequestId = requestId,
            ProjectName = "Updated Project Name",
            ProjectDescription = "Updated description",
            UserId = Guid.NewGuid()
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/InfraRequests/{requestId}/basic-info", 
            updateCommand, 
            _jsonOptions);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task SaveRequestAsDraft_DraftRequest_Success() {
        // Arrange
        var requestId = await CreateTestRequest();
        var userId = Guid.NewGuid();

        // Act
        var response = await _client.PostAsync(
            $"/api/InfraRequests/{requestId}/save-draft?userId={userId}", 
            null);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    #endregion

    #region Helper Methods

    private async Task<Guid> CreateTestRequest() {
        var command = new {
            ProjectName = $"Test Project {Guid.NewGuid().ToString()[..8]}",
            ProjectDescription = "Test",
            SectorId = Guid.NewGuid(),
            SubSectorId = Guid.NewGuid(),
            AssetTypeId = Guid.NewGuid(),
            TenderingStage = TenderingStage.PreTender,
            FundingModel = FundingModel.FullyGovernmentFunded,
            CreatedBy = Guid.NewGuid(),
            CompanyName = "TestCo"
        };

        var response = await _client.PostAsJsonAsync("/api/InfraRequests", command, _jsonOptions);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(_jsonOptions);
        return body!.Data;
    }

    private async Task<Guid> CreateTestRequestForUser(Guid userId) {
        var command = new {
            ProjectName = $"User Test {Guid.NewGuid().ToString()[..8]}",
            ProjectDescription = "Test",
            SectorId = Guid.NewGuid(),
            SubSectorId = Guid.NewGuid(),
            AssetTypeId = Guid.NewGuid(),
            TenderingStage = TenderingStage.PreTender,
            FundingModel = FundingModel.FullyGovernmentFunded,
            CreatedBy = userId,
            CompanyName = "TestCo"
        };

        var response = await _client.PostAsJsonAsync("/api/InfraRequests", command, _jsonOptions);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(_jsonOptions);
        return body!.Data;
    }

    private async Task<Guid> CreateCompleteTestRequest() {
        var requestId = await CreateTestRequest();
        var itemId = await AddTestItem(requestId);
        await AddTestDistribution(requestId, itemId);
        return requestId;
    }

    private async Task<Guid> AddTestItem(Guid requestId) {
        var command = new {
            RequestId = requestId,
            ItemCode = $"ITM-{Guid.NewGuid().ToString()[..8]}",
            ItemName = "Test Item",
            UomId = Guid.NewGuid(),
            Quantity = 10m,
            UnitPrice = 1000m
        };

        var response = await _client.PostAsJsonAsync($"/api/InfraRequests/{requestId}/items", command, _jsonOptions);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(_jsonOptions);
        return body!.Data;
    }

    private async Task<Guid> AddTestDistribution(Guid requestId, Guid itemId) {
        var command = new {
            RequestId = requestId,
            ItemId = itemId,
            AmountType = AmountType.CAPEX,
            Year = 1,
            Amount = 10000m
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/InfraRequests/{requestId}/items/{itemId}/distributions", 
            command, 
            _jsonOptions);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(_jsonOptions);
        return body!.Data;
    }

    private async Task<Guid> AddTestAttachment(Guid requestId) {
        var command = new {
            RequestId = requestId,
            FileName = "test.pdf",
            FileSizeInBytes = 1024000L,
            ContentType = "application/pdf",
            SharePointFileId = $"SP-{Guid.NewGuid()}",
            SharePointUrl = "https://test.com/file.pdf",
            SharePointLibrary = "InfraBase",
            UploadedBy = Guid.NewGuid()
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/InfraRequests/{requestId}/attachments", 
            command, 
            _jsonOptions);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>(_jsonOptions);
        return body!.Data;
    }

    private async Task SubmitTestRequest(Guid requestId) {
        await _client.PostAsync($"/api/InfraRequests/{requestId}/submit?userId={Guid.NewGuid()}", null);
    }

    #endregion
}
