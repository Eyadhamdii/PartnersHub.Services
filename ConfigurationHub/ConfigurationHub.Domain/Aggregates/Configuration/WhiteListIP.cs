using ConfigurationHub.Domain.ValueObjects;
using PartnersHub.ConfigurationHub.Domain.Common;

namespace PartnersHub.ConfigurationHub.Domain.Aggregates.Configuration;

/// <summary>
/// Represents an IP address that is whitelisted for accessing PIFComp features
/// </summary>
public class WhiteListIP : AggregateRoot {
    public IPAddress IPAddress { get; private set; } = null!;
    public DateTime ExpiryDate { get; private set; }
    public bool IsActive { get; private set; }
    public string? Description { get; private set; }

    // EF Core constructor
    private WhiteListIP() { }

    private WhiteListIP(
        IPAddress ipAddress,
        DateTime expiryDate,
        string? description,
        Guid createdBy) {
        IPAddress = ipAddress;
        ExpiryDate = expiryDate;
        Description = description;
        IsActive = true;
        MarkAsCreated(createdBy);
    }

    public static Result<WhiteListIP> Create(
        string ipAddressValue,
        DateTime expiryDate,
        string? description,
        Guid createdBy) {
        if (string.IsNullOrWhiteSpace(ipAddressValue))
            return Result<WhiteListIP>.Failure("IP address is required");

        var ipAddressResult = IPAddress.Create(ipAddressValue);
        if (ipAddressResult.IsFailure)
            return Result<WhiteListIP>.Failure(ipAddressResult.Error!);

        if (expiryDate <= DateTime.UtcNow)
            return Result<WhiteListIP>.Failure("Expiry date must be in the future");

        var whitelistIp = new WhiteListIP(
            ipAddressResult.Value!,
            expiryDate,
            description?.Trim(),
            createdBy);

        return Result<WhiteListIP>.Success(whitelistIp);
    }

    public Result Update(string? description, DateTime? expiryDate, Guid updatedBy) {
        if (description != null)
            Description = description.Trim();

        if (expiryDate.HasValue) {
            if (expiryDate.Value <= DateTime.UtcNow)
                return Result.Failure("Expiry date must be in the future");

            ExpiryDate = expiryDate.Value;
        }

        MarkAsUpdated(updatedBy);
        return Result.Success();
    }

    public Result Activate(Guid updatedBy) {
        if (IsActive)
            return Result.Failure("IP is already active");

        IsActive = true;
        MarkAsUpdated(updatedBy);
        return Result.Success();
    }

    public Result Deactivate(Guid updatedBy) {
        if (!IsActive)
            return Result.Failure("IP is already inactive");

        IsActive = false;
        MarkAsUpdated(updatedBy);
        return Result.Success();
    }

    public bool IsExpired() => ExpiryDate <= DateTime.UtcNow;

    public bool IsValid() => IsActive && !IsExpired();
}