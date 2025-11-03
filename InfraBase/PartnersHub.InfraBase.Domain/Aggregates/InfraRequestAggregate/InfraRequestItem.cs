using PartnersHub.InfraBase.Domain.Common;
using PartnersHub.InfraBase.Domain.Enums;
using PartnersHub.InfraBase.Domain.ValueObjects;

namespace PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;

/// <summary>
/// Represents an item within an InfraRequest
/// Can only be created through the InfraRequest aggregate root
/// </summary>
public class InfraRequestItem : Entity {

    public string ItemCode { get; private set; } = null!;
    public ItemName ItemName { get; private set; } = null!;
    public Guid UomId { get; private set; }
    public Quantity Quantity { get; private set; } = null!;
    public UnitPrice UnitPrice { get; private set; } = null!;
    public decimal TotalAmount => Quantity.Value * UnitPrice.Value;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<InfraRequestItemFinancialDistribution> FinancialDistributions => _financialDistributions.AsReadOnly();
    private readonly List<InfraRequestItemFinancialDistribution> _financialDistributions = new();

    private InfraRequestItem() { }

    internal InfraRequestItem(string itemCode, string itemName, Guid uomId, decimal quantity, decimal unitPrice) {
        var itemNameResult = ItemName.Create(itemName);
        if (itemNameResult.IsFailure) {
            throw new ArgumentException(itemNameResult.Error);
        }

        var quantityResult = Quantity.Create(quantity);
        if (quantityResult.IsFailure) {
            throw new ArgumentException(quantityResult.Error);
        }

        var unitPriceResult = UnitPrice.Create(unitPrice);
        if (unitPriceResult.IsFailure) {
            throw new ArgumentException(unitPriceResult.Error);
        }

        ItemCode = itemCode;
        ItemName = itemNameResult.Value!;
        UomId = uomId;
        Quantity = quantityResult.Value!;
        UnitPrice = unitPriceResult.Value!;
        CreatedAt = DateTime.UtcNow;
    }

    public Result<bool> UpdateQuantity(decimal newQuantity) {
        var quantityResult = Quantity.Create(newQuantity);
        if (quantityResult.IsFailure) {
            return Result<bool>.Failure(quantityResult.Error!);
        }

        Quantity = quantityResult.Value!;
        UpdatedAt = DateTime.UtcNow;
        return Result<bool>.Success(true);
    }

    public Result<bool> UpdateUnitPrice(decimal newUnitPrice) {
        var unitPriceResult = UnitPrice.Create(newUnitPrice);
        if (unitPriceResult.IsFailure) {
            return Result<bool>.Failure(unitPriceResult.Error!);
        }

        UnitPrice = unitPriceResult.Value!;
        UpdatedAt = DateTime.UtcNow;
        return Result<bool>.Success(true);
    }

    public Result<bool> AddFinancialDistribution(AmountType amountType, int year, decimal amount) {
        try {
            var distribution = new InfraRequestItemFinancialDistribution(amountType, year, amount);
            _financialDistributions.Add(distribution);
            return Result<bool>.Success(true);
        }
        catch (ArgumentException ex) {
            return Result<bool>.Failure(ex.Message);
        }
    }

    public Result<bool> ValidateFinancialDistributions() {
        var totalDistributed = _financialDistributions.Sum(fd => fd.Amount.Value);

        if (Math.Abs(totalDistributed - TotalAmount) > 0.01m) {
            return Result<bool>.Failure("Total financial distributions must equal the total amount");
        }

        return Result<bool>.Success(true);
    }

    public void RemoveFinancialDistribution(Guid distributionId) {
        var distribution = _financialDistributions.FirstOrDefault(fd => fd.Id == distributionId);
        if (distribution != null) {
            _financialDistributions.Remove(distribution);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}