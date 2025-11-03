using MediatR;
using PartnersHub.InfraBase.Application.Common.Interfaces;
using PartnersHub.InfraBase.Application.Common.Interfaces.Repository;

namespace PartnersHub.InfraBase.Application.InfraRequests.Queries;

/// <summary>
/// Handler for getting a single request by ID with complete details including lookup descriptions
/// Enriches data with names from ConfigurationHub (Sector, SubSector, AssetType, UOM)
/// </summary>
public class GetRequestByIdQueryHandler : IRequestHandler<GetRequestByIdQuery, RequestDto?> {
    private readonly IInfrabaseRequestRepository _repository;
    private readonly IConfigurationLookupService _lookupService;

    public GetRequestByIdQueryHandler(
        IInfrabaseRequestRepository repository,
        IConfigurationLookupService lookupService) {
        _repository = repository;
        _lookupService = lookupService;
    }

    public async Task<RequestDto?> Handle(GetRequestByIdQuery query, CancellationToken cancellationToken) {
        var request = await _repository.GetByIdAsync(query.RequestId, cancellationToken);

        if (request == null)
            return null;

        // Create base DTO
        var dto = new RequestDto {
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
            
            StartConstructionQuarter = request.StartConstructionQuarter,
            StartConstructionYear = request.StartConstructionYear,
            EndConstructionQuarter = request.EndConstructionQuarter,
            EndConstructionYear = request.EndConstructionYear,
            
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
            }).ToList(),
            
            Attachments = request.GetAttachments().Select(a => new AttachmentDto {
                Id = a.Id,
                FileName = a.Metadata.FileName,
                FileSizeInBytes = a.Metadata.FileSizeInBytes,
                ContentType = a.Metadata.ContentType,
                SharePointFileId = a.SharePointFileId,
                SharePointUrl = a.SharePointUrl,
                UploadedBy = a.UploadedBy,
                UploadedAt = a.UploadedAt
            }).ToList()
        };

        // Enrich with lookup names from ConfigurationHub
        return await EnrichWithLookupNames(dto, cancellationToken);
    }

    private async Task<RequestDto> EnrichWithLookupNames(RequestDto dto, CancellationToken cancellationToken) {
        // Fetch lookup names in parallel
        var sectorTask = _lookupService.GetSectorNameAsync(dto.SectorId, cancellationToken);
        var subSectorTask = _lookupService.GetSubSectorNameAsync(dto.SubSectorId, cancellationToken);
        var assetTypeTask = _lookupService.GetAssetTypeNameAsync(dto.AssetTypeId, cancellationToken);

        // Get unique UOM IDs from items
        var uomIds = dto.Items.Select(i => i.UomId).Distinct().ToList();
        var uomTasks = uomIds.ToDictionary(
            id => id,
            id => _lookupService.GetUomNameAsync(id, cancellationToken));

        // Wait for all lookup tasks
        await Task.WhenAll(
            new Task[] { sectorTask, subSectorTask, assetTypeTask }
            .Concat(uomTasks.Values));

        // Create enriched items with UOM names
        var enrichedItems = dto.Items.Select(item => {
            var uomName = uomTasks.TryGetValue(item.UomId, out var uomTask) ? uomTask.Result : null;
            return item with { UomName = uomName };
        }).ToList();

        // Return DTO with lookup names populated
        return dto with {
            SectorName = sectorTask.Result,
            SubSectorName = subSectorTask.Result,
            AssetTypeName = assetTypeTask.Result,
            Items = enrichedItems
        };
    }
}