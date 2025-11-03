using MediatR;
using PartnersHub.InfraBase.Domain.Enums;

namespace PartnersHub.InfraBase.Application.InfraRequests.Commands;

/// <summary>
/// Command to update an item in a request including its financial distributions
/// Financial distributions use "replace all" approach - existing distributions are removed and new ones added
/// </summary>
public record UpdateRequestItemCommand : IRequest<bool>
{
    public Guid RequestId { get; init; }
    public Guid ItemId { get; init; }
    public Guid UserId { get; init; }  // Added for history tracking
    public decimal? Quantity { get; init; }
    public decimal? UnitPrice { get; init; }
    
    /// <summary>
    /// Financial distributions for this item
    /// All existing distributions will be removed and replaced with these
    /// </summary>
    public List<UpdateItemFinancialDistributionDto> FinancialDistributions { get; init; } = new();
}

/// <summary>
/// DTO for financial distribution in update item command
/// </summary>
public record UpdateItemFinancialDistributionDto
{
    public AmountType AmountType { get; init; }
    /// <summary>
    /// Year for this distribution. Accepts:
    /// - Offset (1-99): Year 1, Year 2, etc.
    /// - Full year (2000-2099): 2024, 2025, etc.
    /// </summary>
    public int Year { get; init; }
    public decimal Amount { get; init; }
}
