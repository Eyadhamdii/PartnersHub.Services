using PartnersHub.InfraBase.Domain.Common;
using PartnersHub.InfraBase.Domain.Enums;
using PartnersHub.InfraBase.Domain.ValueObjects;

namespace PartnersHub.InfraBase.Domain.Aggregates.InfraRequestAggregate;

/// <summary>
/// Represents a financial distribution for an item
/// Can only be created through InfraRequestItem
/// </summary>
public class InfraRequestItemFinancialDistribution : Entity {
    public Guid RequestItemId { get; private set; }
    public AmountType AmountType { get; private set; }
    public int Year { get; private set; }
    public YearlyAmount Amount { get; private set; } = null!;

    private InfraRequestItemFinancialDistribution() { }

    internal InfraRequestItemFinancialDistribution(AmountType amountType, int year, decimal amount) {
        if (year >= 3000 || year < 2000) {
            throw new ArgumentException("Year must be between 2000-2099");
        }

        var yearlyAmountResult = YearlyAmount.Create(amount);
        if (yearlyAmountResult.IsFailure) {
            throw new ArgumentException(yearlyAmountResult.Error);
        }

        AmountType = amountType;
        Year = year;
        Amount = yearlyAmountResult.Value!;
    }

    public Result<bool> UpdateAmount(decimal newAmount) {
        var amountResult = YearlyAmount.Create(newAmount);
        if (amountResult.IsFailure) {
            return Result<bool>.Failure(amountResult.Error!);
        }

        Amount = amountResult.Value!;
        return Result<bool>.Success(true);
    }
}