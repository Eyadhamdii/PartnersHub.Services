using Microsoft.EntityFrameworkCore;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;
using PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;
using PartnersHub.InfraBase.Domain.Enums;
using PartnersHub.InfraBase.Infrastructure.Persistence;
using PartnersHub.InfraBase.Infrastructure.Persistence.Repositories;

namespace PartnersHub.Services.InfraBase.IntegrationTests;

/// <summary>
/// Integration tests for InfraBase - tests the full stack with real database operations
/// Uses in-memory database for fast, isolated testing
/// </summary>
public class InfraRequestIntegrationTests {
    private InfrabaseDbContext _context = null!;
    private IInfrabaseRequestRepository _repository = null!;
    private IUnitOfWork _unitOfWork = null!;

    [SetUp]
    public void Setup() {
        // Create in-memory database
        var options = new DbContextOptionsBuilder<InfrabaseDbContext>()
            .UseInMemoryDatabase(databaseName: $"InfraBaseTestDb_{Guid.NewGuid()}")
            .Options;

        _context = new InfrabaseDbContext(options);
        _repository = new InfrabaseRequestRepository(_context);
        _unitOfWork = new UnitOfWork(_context);
    }

    [TearDown]
    public void TearDown() {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region Create Request Tests

    [Test]
    public async Task CreateRequest_SavesToDatabase_Successfully() {
        var sectorId = Guid.NewGuid();
        var subSectorId = Guid.NewGuid();
        var assetTypeId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var requestResult = InfraRequest.Create(
            projectName: "Integration Test Project",
            projectDescription: "Testing full database integration",
            sectorId: sectorId,
            subSectorId: subSectorId,
            assetTypeId: assetTypeId,
            assetTypeOtherDescription: null,
            tenderingStage: TenderingStage.PreTender,
            fundingModel: FundingModel.FullyGovernmentFunded,
            createdBy: userId,
            companyId: null,
            companyName: "Test Company"
        );

        var request = requestResult.Value!;

        await _repository.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(request.Id);
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.ProjectName.Value, Is.EqualTo("Integration Test Project"));
        Assert.That(saved.Status, Is.EqualTo(RequestStatus.Draft));
        Assert.That(saved.SectorId, Is.EqualTo(sectorId));
    }

    [Test]
    public async Task CreateRequest_WithItems_SavesCompleteAggregate() {
        var request = CreateTestRequest();
        var uomId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        request.AddItem("ITM-001", "Test Equipment", uomId, 10, 100000, userId);

        await _repository.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        var saved = await _context.InfraRequests
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == request.Id);

        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Items.Count, Is.EqualTo(1));
        Assert.That(saved.Items.First().ItemCode, Is.EqualTo("ITM-001"));
    }

    [Test]
    public async Task CreateRequest_WithItemsAndDistributions_SavesCompleteStructure() {
        var request = CreateTestRequest();
        var uomId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        request.AddItem("ITM-001", "Test Item", uomId, 10, 100000, userId);
        var item = request.Items.First();
        item.AddFinancialDistribution(AmountType.CAPEX, 2025, 600000);
        item.AddFinancialDistribution(AmountType.CAPEX, 2026, 400000);

        await _repository.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        var saved = await _context.InfraRequests
            .Include(r => r.Items)
            .ThenInclude(i => i.FinancialDistributions)
            .FirstOrDefaultAsync(r => r.Id == request.Id);

        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Items.Count, Is.EqualTo(1));
        Assert.That(saved.Items.First().FinancialDistributions.Count, Is.EqualTo(2));
    }

    #endregion

    #region Workflow Tests

    [Test]
    public async Task SubmitRequest_ChangesStatusAndCreatesHistory() {
        // Create request without code
        var request = CreateCompleteTestRequest();
        await _repository.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        var requestId = request.Id;
        var userId = Guid.NewGuid();
        var requestCode = await _repository.GetNextRequestCodeAsync();

        // Load fresh and submit with new code (simulating the API call)
        _context.ChangeTracker.Clear();
        var savedRequest = await _context.InfraRequests
            .Include(r => r.Items)
            .ThenInclude(i => i.FinancialDistributions)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        var result = savedRequest!.Submit(userId, requestCode);
        await _unitOfWork.SaveChangesAsync();

        Assert.That(result.IsSuccess, Is.True);

        _context.ChangeTracker.Clear();
        var finalRequest = await _context.InfraRequests
            .Include(r => r.History)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == requestId);

        Assert.That(finalRequest, Is.Not.Null);
        Assert.That(finalRequest!.Status, Is.EqualTo(RequestStatus.PendingPcAdminApproval));
        Assert.That(finalRequest.RequestCode, Is.Not.Null);
        Assert.That(finalRequest.History.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task AcceptByPcAdmin_ChangesStatusCorrectly() {
        var request = CreateCompleteTestRequest();
        var submitterId = Guid.NewGuid();
        var requestCode = await _repository.GetNextRequestCodeAsync();
        request.AssignRequestCode(requestCode);
        request.Submit(submitterId, requestCode);

        await _repository.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        var requestId = request.Id;
        var pcAdminId = Guid.NewGuid();

        _context.ChangeTracker.Clear();
        var savedRequest = await _context.InfraRequests
            .FirstOrDefaultAsync(r => r.Id == requestId);

        var result = savedRequest!.AcceptByPcAdmin(pcAdminId);
        await _unitOfWork.SaveChangesAsync();

        Assert.That(result.IsSuccess, Is.True);

        _context.ChangeTracker.Clear();
        var finalRequest = await _context.InfraRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == requestId);

        Assert.That(finalRequest, Is.Not.Null);
        Assert.That(finalRequest!.Status, Is.EqualTo(RequestStatus.PendingPcInfrabaseApproval));
    }

    [Test]
    public async Task RejectByPcAdmin_ChangesStatusAndSavesReason() {
        var request = CreateCompleteTestRequest();
        var requestCode = await _repository.GetNextRequestCodeAsync();
        request.AssignRequestCode(requestCode);
        request.Submit(Guid.NewGuid(), requestCode);

        await _repository.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        var requestId = request.Id;
        var pcAdminId = Guid.NewGuid();
        var rejectionReason = "Project scope is not clear";

        _context.ChangeTracker.Clear();
        var savedRequest = await _context.InfraRequests
            .FirstOrDefaultAsync(r => r.Id == requestId);

        var result = savedRequest!.RejectByPcAdmin(pcAdminId, rejectionReason);
        await _unitOfWork.SaveChangesAsync();

        Assert.That(result.IsSuccess, Is.True);

        _context.ChangeTracker.Clear();
        var finalRequest = await _context.InfraRequests
            .Include(r => r.History)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == requestId);

        Assert.That(finalRequest, Is.Not.Null);
        Assert.That(finalRequest!.Status, Is.EqualTo(RequestStatus.RejectedByPcAdmin));
        Assert.That(finalRequest.RejectionReason, Is.Not.Null);
    }

    [Test]
    public async Task CompleteApprovalWorkflow_GoesThoughAllStates() {
        var request = CreateCompleteTestRequest();
        var userId = Guid.NewGuid();
        var pcAdminId = Guid.NewGuid();
        var infraAdminId = Guid.NewGuid();

        await _repository.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        var requestId = request.Id;
        var requestCode = await _repository.GetNextRequestCodeAsync();

        _context.ChangeTracker.Clear();
        var step1Request = await _context.InfraRequests
            .Include(r => r.Items)
            .ThenInclude(i => i.FinancialDistributions)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        step1Request!.AssignRequestCode(requestCode);
        var submitResult = step1Request.Submit(userId, requestCode);
        await _unitOfWork.SaveChangesAsync();
        Assert.That(submitResult.IsSuccess, Is.True);

        _context.ChangeTracker.Clear();
        var afterSubmit = await _context.InfraRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == requestId);
        Assert.That(afterSubmit!.Status, Is.EqualTo(RequestStatus.PendingPcAdminApproval));

        _context.ChangeTracker.Clear();
        var step2Request = await _context.InfraRequests
            .FirstOrDefaultAsync(r => r.Id == requestId);

        var acceptResult = step2Request!.AcceptByPcAdmin(pcAdminId);
        await _unitOfWork.SaveChangesAsync();
        Assert.That(acceptResult.IsSuccess, Is.True);

        _context.ChangeTracker.Clear();
        var afterAccept = await _context.InfraRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == requestId);
        Assert.That(afterAccept!.Status, Is.EqualTo(RequestStatus.PendingPcInfrabaseApproval));

        _context.ChangeTracker.Clear();
        var step3Request = await _context.InfraRequests
            .FirstOrDefaultAsync(r => r.Id == requestId);

        var approveResult = step3Request!.AcceptByInfrabaseAdmin(infraAdminId);
        await _unitOfWork.SaveChangesAsync();
        Assert.That(approveResult.IsSuccess, Is.True);

        _context.ChangeTracker.Clear();
        var finalRequest = await _context.InfraRequests
            .Include(r => r.History)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == requestId);

        Assert.That(finalRequest, Is.Not.Null);
        Assert.That(finalRequest!.Status, Is.EqualTo(RequestStatus.InfrabaseApproved));
        Assert.That(finalRequest.History.Count, Is.GreaterThanOrEqualTo(3));
    }

    #endregion

    #region Query Tests

    [Test]
    public async Task GetByStatus_ReturnsCorrectRequests() {
        var request1 = CreateCompleteTestRequest();
        var request2 = CreateCompleteTestRequest();
        var request3 = CreateCompleteTestRequest();

        var code1 = await _repository.GetNextRequestCodeAsync();
        request1.AssignRequestCode(code1);
        request1.Submit(Guid.NewGuid(), code1);

        var code2 = await _repository.GetNextRequestCodeAsync();
        request2.AssignRequestCode(code2);
        request2.Submit(Guid.NewGuid(), code2);
        request2.AcceptByPcAdmin(Guid.NewGuid());

        var code3 = await _repository.GetNextRequestCodeAsync();
        request3.AssignRequestCode(code3);
        request3.Submit(Guid.NewGuid(), code3);

        await _repository.AddAsync(request1);
        await _repository.AddAsync(request2);
        await _repository.AddAsync(request3);
        await _unitOfWork.SaveChangesAsync();

        _context.ChangeTracker.Clear();
        var pendingPcAdmin = await _context.InfraRequests
            .Where(r => r.Status == RequestStatus.PendingPcAdminApproval)
            .ToListAsync();

        var pendingInfraAdmin = await _context.InfraRequests
            .Where(r => r.Status == RequestStatus.PendingPcInfrabaseApproval)
            .ToListAsync();

        Assert.That(pendingPcAdmin.Count, Is.EqualTo(2));
        Assert.That(pendingInfraAdmin.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetByUser_ReturnsOnlyUserRequests() {
        var user1Id = Guid.NewGuid();
        var user2Id = Guid.NewGuid();

        var request1 = CreateTestRequestForUser(user1Id);
        var request2 = CreateTestRequestForUser(user1Id);
        var request3 = CreateTestRequestForUser(user2Id);

        await _repository.AddAsync(request1);
        await _repository.AddAsync(request2);
        await _repository.AddAsync(request3);
        await _unitOfWork.SaveChangesAsync();

        var user1Requests = await _context.InfraRequests
            .Where(r => r.CreatedBy == user1Id)
            .ToListAsync();

        Assert.That(user1Requests.Count, Is.EqualTo(2));
        Assert.That(user1Requests.All(r => r.CreatedBy == user1Id), Is.True);
    }

    #endregion

    #region Delete Tests

    [Test]
    public async Task RemoveItem_DeletesFromDatabase() {
        var request = CreateCompleteTestRequest();
        await _repository.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        var userId = Guid.NewGuid();
        _context.ChangeTracker.Clear();
        var savedRequest = await _context.InfraRequests
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == request.Id);

        Assert.That(savedRequest, Is.Not.Null);
        Assert.That(savedRequest!.Items.Count, Is.EqualTo(1));
        var itemId = savedRequest.Items.First().Id;

        // Remove the item
        var result = savedRequest!.RemoveItem(itemId, userId);
        Assert.That(result.IsSuccess, Is.True);

        await _unitOfWork.SaveChangesAsync();

        _context.ChangeTracker.Clear();
        var finalRequest = await _context.InfraRequests
            .Include(r => r.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.Id);

        Assert.That(finalRequest, Is.Not.Null);
        Assert.That(finalRequest!.Items.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task RemoveDistribution_DeletesFromDatabase() {
        var request = CreateCompleteTestRequest();
        await _repository.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        var distId = request.Items.First().FinancialDistributions.First().Id;
        var requestId = request.Id;

        _context.ChangeTracker.Clear();
        var savedRequest = await _context.InfraRequests
            .Include(r => r.Items)
            .ThenInclude(i => i.FinancialDistributions)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        var savedItem = savedRequest!.Items.First();
        savedItem.RemoveFinancialDistribution(distId);
        await _unitOfWork.SaveChangesAsync();

        _context.ChangeTracker.Clear();
        var finalRequest = await _context.InfraRequests
            .Include(r => r.Items)
            .ThenInclude(i => i.FinancialDistributions)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == requestId);

        var finalItem = finalRequest!.Items.First();
        Assert.That(finalItem.FinancialDistributions.Count, Is.EqualTo(0));
    }

    #endregion

    #region Attachment Tests

    [Test]
    public async Task AddAttachment_SavesMetadata() {
        var request = CreateTestRequest();
        await _repository.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        var requestId = request.Id;
        var userId = Guid.NewGuid();

        _context.ChangeTracker.Clear();
        var savedRequest = await _context.InfraRequests
            .FirstOrDefaultAsync(r => r.Id == requestId);

        var result = savedRequest!.AddAttachment(
            fileName: "test.pdf",
            fileSizeInBytes: 1024000,
            contentType: "application/pdf",
            sharePointFileId: "SP-123",
            sharePointUrl: "https://sharepoint.test.com/file.pdf",
            sharePointLibrary: "InfraBase",
            uploadedBy: userId
        );
        
        // Attachment is already tracked through aggregate - just save
        await _unitOfWork.SaveChangesAsync();

        Assert.That(result.IsSuccess, Is.True);

        _context.ChangeTracker.Clear();
        var finalRequest = await _context.InfraRequests
            .Include(r => r.Attachments)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == requestId);

        Assert.That(finalRequest, Is.Not.Null);
        Assert.That(finalRequest!.Attachments.Count, Is.EqualTo(1));
        Assert.That(finalRequest.Attachments.First().IsDeleted, Is.False);
    }

    [Test]
    public async Task RemoveAttachment_SoftDeletes() {
        var request = CreateTestRequest();
        var userId = Guid.NewGuid();
        var attachmentResult = request.AddAttachment(
            "test.pdf", 1024, "application/pdf", "SP-1",
            "https://test.com", "Lib", userId
        );

        await _repository.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        var attachmentId = attachmentResult.Value!.Id;
        var requestId = request.Id;

        _context.ChangeTracker.Clear();
        var savedRequest = await _context.InfraRequests
            .Include(r => r.Attachments)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        var removeResult = savedRequest!.RemoveAttachment(attachmentId, userId);
        await _unitOfWork.SaveChangesAsync();

        Assert.That(removeResult.IsSuccess, Is.True);

        _context.ChangeTracker.Clear();
        var finalRequest = await _context.InfraRequests
            .Include(r => r.Attachments)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == requestId);

        var savedAttachment = finalRequest!.Attachments.First();
        Assert.That(savedAttachment.IsDeleted, Is.True);
        Assert.That(savedAttachment.DeletedBy, Is.EqualTo(userId));
        Assert.That(savedAttachment.DeletedAt, Is.Not.Null);
    }

    #endregion

    #region Helper Methods

    private InfraRequest CreateTestRequest() {
        var result = InfraRequest.Create(
            projectName: $"Test Project {Guid.NewGuid().ToString().Substring(0, 8)}",
            projectDescription: "Integration test project",
            sectorId: Guid.NewGuid(),
            subSectorId: Guid.NewGuid(),
            assetTypeId: Guid.NewGuid(),
            assetTypeOtherDescription: null,
            tenderingStage: TenderingStage.PreTender,
            fundingModel: FundingModel.FullyGovernmentFunded,
            createdBy: Guid.NewGuid(),
            companyId: null,
            companyName: "Test Company"
        );

        return result.Value!;
    }

    private InfraRequest CreateTestRequestForUser(Guid userId) {
        var result = InfraRequest.Create(
            projectName: $"Test Project {Guid.NewGuid().ToString().Substring(0, 8)}",
            projectDescription: "Integration test project",
            sectorId: Guid.NewGuid(),
            subSectorId: Guid.NewGuid(),
            assetTypeId: Guid.NewGuid(),
            assetTypeOtherDescription: null,
            tenderingStage: TenderingStage.PreTender,
            fundingModel: FundingModel.FullyGovernmentFunded,
            createdBy: userId,
            companyId: null,
            companyName: "Test Company"
        );

        return result.Value!;
    }

    private InfraRequest CreateCompleteTestRequest() {
        var request = CreateTestRequest();
        var uomId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        request.AddItem("ITM-001", "Test Item", uomId, 10, 100000, userId);
        var item = request.Items.First();
        item.AddFinancialDistribution(AmountType.CAPEX, 2025, 1000000);

        return request;
    }

    #endregion
}
