using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;
using PartnersHub.InfraBase.Application.Common.Models;

namespace PartnersHub.InfraBase.Application.InfraRequests.Queries;

/// <summary>
/// Handler for getting all requests with pagination
/// </summary>
public class GetAllRequestsQueryHandler : IRequestHandler<GetAllRequestsQuery, PaginatedList<RequestDto>> {
    private readonly IInfrabaseRequestRepository _repository;

    public GetAllRequestsQueryHandler(IInfrabaseRequestRepository repository) {
        _repository = repository;
    }

    public async Task<PaginatedList<RequestDto>> Handle(GetAllRequestsQuery query, CancellationToken cancellationToken) {
        var (items, totalCount) = query.Status.HasValue
            ? await _repository.GetByStatusPaginatedAsync(query.Status.Value, query.PageNumber, query.PageSize, cancellationToken)
            : await _repository.GetAllPaginatedAsync(query.PageNumber, query.PageSize, cancellationToken);

        var dtos = items.Select(MapToDto).ToList();

        return PaginatedList<RequestDto>.Create(dtos, totalCount, query.PageNumber, query.PageSize);
    }

    private RequestDto MapToDto(Domain.Aggregates.InfraRequestAggregate.InfraRequest request) {
        return new RequestDto {
            Id = request.Id,
            RequestCode = request.RequestCode,
            ProjectName = request.ProjectName.Value,
            ProjectDescription = request.ProjectDescription?.Value,
            SectorId = request.SectorId,
            SubSectorId = request.SubSectorId,
            AssetTypeId = request.AssetTypeId,
            AssetTypeOtherDescription = request.AssetTypeOtherDescription,
            TenderingStage = request.TenderingStage,
            FundingModel = request.FundingModel,
            Status = request.Status,
            TotalRequestAmount = request.TotalRequestAmount,
            CreatedBy = request.CreatedBy,
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt,
            SubmittedAt = request.SubmittedAt,
            RejectionReason = request.RejectionReason?.Value,
            RejectedBy = request.RejectedBy,
            RejectedAt = request.RejectedAt,
            ApprovedBy = request.ApprovedBy,
            ApprovedAt = request.ApprovedAt,
            CompanyId = request.CompanyId,
            CompanyName = request.CompanyName,
            Items = request.Items.Select(i => new RequestItemDto {
                Id = i.Id,
                ItemCode = i.ItemCode,
                ItemName = i.ItemName.Value,
                UomId = i.UomId,
                Quantity = i.Quantity.Value,
                UnitPrice = i.UnitPrice.Value,
                TotalAmount = i.TotalAmount,
                FinancialDistributions = i.FinancialDistributions.Select(fd => new FinancialDistributionDto {
                    Id = fd.Id,
                    AmountType = fd.AmountType,
                    Year = fd.Year,
                    Amount = fd.Amount.Value
                }).ToList()
            }).ToList(),
            History = request.History.OrderBy(h => h.PerformedAt).Select(h => new RequestHistoryDto {
                Id = h.Id,
                Status = h.Status,
                Action = h.Action,
                Comments = h.Comments,
                PerformedBy = h.PerformedBy,
                PerformedAt = h.PerformedAt,
                FieldsChanged = h.FieldsChanged,
                OldValues = h.OldValues,
                NewValues = h.NewValues
            }).ToList()
        };
    }
}

/// <summary>
/// Handler for getting requests by user with pagination
/// </summary>
public class GetRequestsByUserQueryHandler : IRequestHandler<GetRequestsByUserQuery, PaginatedList<RequestDto>> {
    private readonly IInfrabaseRequestRepository _repository;

    public GetRequestsByUserQueryHandler(IInfrabaseRequestRepository repository) {
        _repository = repository;
    }

    public async Task<PaginatedList<RequestDto>> Handle(GetRequestsByUserQuery query, CancellationToken cancellationToken) {
        var (items, totalCount) = await _repository.GetByCreatedByPaginatedAsync(
            query.UserId, 
            query.PageNumber, 
            query.PageSize, 
            cancellationToken);

        var dtos = items.Select(MapToDto).ToList();

        return PaginatedList<RequestDto>.Create(dtos, totalCount, query.PageNumber, query.PageSize);
    }

    private RequestDto MapToDto(Domain.Aggregates.InfraRequestAggregate.InfraRequest request) {
        return new RequestDto {
            Id = request.Id,
            RequestCode = request.RequestCode,
            ProjectName = request.ProjectName.Value,
            ProjectDescription = request.ProjectDescription?.Value,
            SectorId = request.SectorId,
            SubSectorId = request.SubSectorId,
            AssetTypeId = request.AssetTypeId,
            AssetTypeOtherDescription = request.AssetTypeOtherDescription,
            TenderingStage = request.TenderingStage,
            FundingModel = request.FundingModel,
            Status = request.Status,
            TotalRequestAmount = request.TotalRequestAmount,
            CreatedBy = request.CreatedBy,
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt,
            SubmittedAt = request.SubmittedAt,
            RejectionReason = request.RejectionReason?.Value,
            RejectedBy = request.RejectedBy,
            RejectedAt = request.RejectedAt,
            ApprovedBy = request.ApprovedBy,
            ApprovedAt = request.ApprovedAt,
            CompanyId = request.CompanyId,
            CompanyName = request.CompanyName,
            Items = request.Items.Select(i => new RequestItemDto {
                Id = i.Id,
                ItemCode = i.ItemCode,
                ItemName = i.ItemName.Value,
                UomId = i.UomId,
                Quantity = i.Quantity.Value,
                UnitPrice = i.UnitPrice.Value,
                TotalAmount = i.TotalAmount,
                FinancialDistributions = i.FinancialDistributions.Select(fd => new FinancialDistributionDto {
                    Id = fd.Id,
                    AmountType = fd.AmountType,
                    Year = fd.Year,
                    Amount = fd.Amount.Value
                }).ToList()
            }).ToList(),
            History = request.History.OrderBy(h => h.PerformedAt).Select(h => new RequestHistoryDto {
                Id = h.Id,
                Status = h.Status,
                Action = h.Action,
                Comments = h.Comments,
                PerformedBy = h.PerformedBy,
                PerformedAt = h.PerformedAt,
                FieldsChanged = h.FieldsChanged,
                OldValues = h.OldValues,
                NewValues = h.NewValues
            }).ToList()
        };
    }
}
