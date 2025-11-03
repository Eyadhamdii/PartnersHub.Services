using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PartnersHub.InfraBase.Apis.Common;
using PartnersHub.InfraBase.Apis.Controllers;
using PartnersHub.InfraBase.Application.Common.Models;
using PartnersHub.InfraBase.Application.InfraRequests.Commands;
using PartnersHub.InfraBase.Application.InfraRequests.Queries;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.Services.InfraBase.Apis.Tests;

public class InfraRequestsControllerTests {
    private Mock<IMediator> _mediatorMock = null!;
    private Mock<ILogger<InfraRequestsController>> _loggerMock = null!;
    private InfraRequestsController _controller = null!;

    [SetUp]
    public void Setup() {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<InfraRequestsController>>();
        _controller = new InfraRequestsController(_mediatorMock.Object, _loggerMock.Object);
    }

    #region Create Request Tests

    [Test]
    public async Task CreateRequest_WithValidCommand_ReturnsCreatedResult() {
        // Arrange
        var command = new CreateRequestCommand {
            ProjectName = "Test Project",
            ProjectDescription = "Test Description",
            SectorId = Guid.NewGuid(),
            SubSectorId = Guid.NewGuid(),
            AssetTypeId = Guid.NewGuid(),
            TenderingStage = TenderingStage.PreTender,
            FundingModel = FundingModel.FullyGovernmentFunded,
            CreatedBy = Guid.NewGuid()
        };

        var expectedId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _controller.CreateRequest(command);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult!.Value, Is.Not.Null);
    }

    [Test]
    public async Task CreateRequest_CallsMediator_Once() {
        // Arrange
        var command = new CreateRequestCommand {
            ProjectName = "Test",
            CreatedBy = Guid.NewGuid()
        };
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        await _controller.CreateRequest(command);

        // Assert
        _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Get Requests Tests

    [Test]
    public async Task GetAllRequests_WithoutStatus_ReturnsOkResult() {
        // Arrange
        var paginatedList = PaginatedList<RequestDto>.Create(new List<RequestDto>(), 0, 1, 10);

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllRequestsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _controller.GetAllRequests();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllRequestsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetAllRequests_WithStatus_PassesStatusToQuery() {
        // Arrange
        var status = "PendingPcAdminApproval";
        GetAllRequestsQuery? capturedQuery = null;

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllRequestsQuery>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<PaginatedList<RequestDto>>, CancellationToken>((q, _) =>
                capturedQuery = q as GetAllRequestsQuery)
            .ReturnsAsync(PaginatedList<RequestDto>.Create(new List<RequestDto>(), 0, 1, 10));

        // Act
        await _controller.GetAllRequests(status);

        // Assert
        Assert.That(capturedQuery, Is.Not.Null);
        Assert.That(capturedQuery!.Status, Is.Not.Null);
    }

    [Test]
    public async Task GetRequestsByUser_WithValidUserId_ReturnsOkResult() {
        // Arrange
        var userId = Guid.NewGuid();
        var paginatedList = PaginatedList<RequestDto>.Create(new List<RequestDto>(), 0, 1, 10);

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetRequestsByUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _controller.GetRequestsByUser(userId);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        _mediatorMock.Verify(m => m.Send(
            It.Is<GetRequestsByUserQuery>(q => q.UserId == userId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Workflow Tests

    [Test]
    public async Task SubmitRequest_WithValidData_ReturnsOkResult() {
        // Arrange
        var requestId = Guid.NewGuid();
        var command = new SubmitRequestCommand {
            RequestId = requestId,
            UserId = Guid.NewGuid()
        };
        var expectedCode = "REQ-001";

        _mediatorMock.Setup(m => m.Send(It.IsAny<SubmitRequestCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCode);

        // Act
        var result = await _controller.SubmitRequest(requestId, command);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task SaveRequestAsDraft_WithValidData_ReturnsOkResult() {
        // Arrange
        var command = new SaveRequestAsDraftCommand {
            UserId = Guid.NewGuid(),
            ProjectName = "Test"
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<SaveRequestAsDraftCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _controller.SaveRequestAsDraft(command);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task AcceptRequestByPcAdmin_WithValidData_CallsMediator() {
        // Arrange
        var requestId = Guid.NewGuid();

        _mediatorMock.Setup(m => m.Send(It.IsAny<AcceptRequestByPcAdminCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _controller.AcceptRequestByPcAdmin(requestId);

        // Assert
        _mediatorMock.Verify(m => m.Send(
            It.Is<AcceptRequestByPcAdminCommand>(c => c.RequestId == requestId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task RejectRequestByPcAdmin_WithMismatchedId_ReturnsBadRequest() {
        // Arrange
        var requestId = Guid.NewGuid();
        var command = new RejectRequestByPcAdminCommand {
            RequestId = Guid.NewGuid(), // Different ID
            UserId = Guid.NewGuid(),
            RejectionReason = "Test reason"
        };

        // Act
        var result = await _controller.RejectRequestByPcAdmin(requestId, command);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        _mediatorMock.Verify(m => m.Send(It.IsAny<RejectRequestByPcAdminCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task RejectRequestByPcAdmin_WithMatchingId_CallsMediator() {
        // Arrange
        var requestId = Guid.NewGuid();
        var command = new RejectRequestByPcAdminCommand {
            RequestId = requestId,
            UserId = Guid.NewGuid(),
            RejectionReason = "Test reason"
        };

        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _controller.RejectRequestByPcAdmin(requestId, command);

        // Assert
        _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Items Tests

    [Test]
    public async Task AddRequestItem_WithValidData_ReturnsCreatedResult() {
        // Arrange
        var requestId = Guid.NewGuid();
        var command = new AddRequestItemCommand {
            RequestId = requestId,
            ItemCode = "ITM-001",
            ItemName = "Test Item",
            UomId = Guid.NewGuid(),
            Quantity = 10,
            UnitPrice = 100
        };
        var expectedItemId = Guid.NewGuid();

        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedItemId);

        // Act
        var result = await _controller.AddRequestItem(requestId, command);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
    }

    [Test]
    public async Task AddRequestItem_WithMismatchedId_ReturnsBadRequest() {
        // Arrange
        var requestId = Guid.NewGuid();
        var command = new AddRequestItemCommand {
            RequestId = Guid.NewGuid(), // Different ID
            ItemName = "Test"
        };

        // Act
        var result = await _controller.AddRequestItem(requestId, command);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateRequestItem_WithValidData_ReturnsOkResult() {
        // Arrange
        var requestId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var command = new UpdateRequestItemCommand {
            RequestId = requestId,
            ItemId = itemId,
            Quantity = 20,
            UnitPrice = 150
        };
        
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateRequestItem(requestId, itemId, command);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task RemoveRequestItem_WithValidIds_CallsMediator() {
        // Arrange
        var requestId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveRequestItemCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _controller.RemoveRequestItem(requestId, itemId);

        // Assert
        _mediatorMock.Verify(m => m.Send(
            It.Is<RemoveRequestItemCommand>(c => c.RequestId == requestId && c.ItemId == itemId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Financial Distributions Tests

    [Test]
    public async Task AddFinancialDistribution_WithValidData_ReturnsCreatedResult() {
        // Arrange
        var requestId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var command = new AddFinancialDistributionCommand {
            RequestId = requestId,
            ItemId = itemId,
            AmountType = AmountType.CAPEX,
            Year = 1,
            Amount = 100000
        };
        var expectedDistId = Guid.NewGuid();
        
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDistId);

        // Act
        var result = await _controller.AddFinancialDistribution(requestId, itemId, command);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
    }

    [Test]
    public async Task AddFinancialDistribution_WithMismatchedIds_ReturnsBadRequest() {
        // Arrange
        var requestId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var command = new AddFinancialDistributionCommand {
            RequestId = Guid.NewGuid(), // Different ID
            ItemId = itemId,
            Year = 1
        };

        // Act
        var result = await _controller.AddFinancialDistribution(requestId, itemId, command);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateFinancialDistribution_WithValidData_ReturnsOkResult() {
        // Arrange
        var requestId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var distId = Guid.NewGuid();
        var command = new UpdateFinancialDistributionCommand {
            RequestId = requestId,
            ItemId = itemId,
            DistributionId = distId,
            Amount = 200000
        };
        
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateFinancialDistribution(requestId, itemId, distId, command);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task RemoveFinancialDistribution_WithValidIds_CallsMediator() {
        // Arrange
        var requestId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var distId = Guid.NewGuid();
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveFinancialDistributionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _controller.RemoveFinancialDistribution(requestId, itemId, distId);

        // Assert
        _mediatorMock.Verify(m => m.Send(
            It.Is<RemoveFinancialDistributionCommand>(c => 
                c.RequestId == requestId && c.ItemId == itemId && c.DistributionId == distId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Attachments Tests

    [Test]
    public async Task AddAttachment_WithValidData_ReturnsCreatedResult() {
        // Arrange
        var requestId = Guid.NewGuid();
        var command = new AddAttachmentCommand {
            RequestId = requestId,
            FileName = "test.pdf",
            FileSizeInBytes = 1024,
            SharePointFileId = "SP-123",
            UploadedBy = Guid.NewGuid()
        };
        var expectedAttachmentId = Guid.NewGuid();
        
        _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAttachmentId);

        // Act
        var result = await _controller.AddAttachment(requestId, command);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
    }

    [Test]
    public async Task GetRequestAttachments_WithValidId_ReturnsOkResult() {
        // Arrange
        var requestId = Guid.NewGuid();
        var attachments = new List<AttachmentDto>();
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetRequestAttachmentsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(attachments);

        // Act
        var result = await _controller.GetRequestAttachments(requestId);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        _mediatorMock.Verify(m => m.Send(
            It.Is<GetRequestAttachmentsQuery>(q => q.RequestId == requestId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task RemoveAttachment_WithValidIds_CallsMediator() {
        // Arrange
        var requestId = Guid.NewGuid();
        var attachmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveAttachmentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _controller.RemoveAttachment(requestId, attachmentId, userId);

        // Assert
        _mediatorMock.Verify(m => m.Send(
            It.Is<RemoveAttachmentCommand>(c => 
                c.RequestId == requestId && c.AttachmentId == attachmentId && c.DeletedBy == userId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
