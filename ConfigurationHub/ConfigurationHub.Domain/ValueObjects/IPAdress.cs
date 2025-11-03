using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PartnersHub.ConfigurationHub.Domain.Common;

namespace ConfigurationHub.Domain.ValueObjects;

/// <summary>
/// Represents an IP address value object with validation
/// </summary>
public sealed class IPAddress : ValueObject {
    public string Value { get; private set; } = null!;

    // EF Core constructor
    private IPAddress() { }

    private IPAddress(string address) {
        Value = address;
    }

    public static Result<IPAddress> Create(string address) {
        if (string.IsNullOrWhiteSpace(address))
            return Result<IPAddress>.Failure("IP address cannot be empty");

        var trimmedAddress = address.Trim();

        // Validate IP address format (IPv4 or IPv6)
        if (!System.Net.IPAddress.TryParse(trimmedAddress, out var parsedAddress))
            return Result<IPAddress>.Failure("Invalid IP address format");

        // Additional validation: Check if it's a valid format
        if (parsedAddress.ToString() != trimmedAddress)
            return Result<IPAddress>.Failure("IP address format is not normalized");

        return Result<IPAddress>.Success(new IPAddress(trimmedAddress));
    }

    /// <summary>
    /// Creates an IP address from CIDR notation (e.g., "192.168.1.0/24")
    /// </summary>
    public static Result<IPAddress> CreateFromCIDR(string cidr) {
        if (string.IsNullOrWhiteSpace(cidr))
            return Result<IPAddress>.Failure("CIDR notation cannot be empty");

        var trimmedCidr = cidr.Trim();

        // Validate CIDR format
        var parts = trimmedCidr.Split('/');
        if (parts.Length != 2)
            return Result<IPAddress>.Failure("Invalid CIDR notation format");

        if (!System.Net.IPAddress.TryParse(parts[0], out _))
            return Result<IPAddress>.Failure("Invalid IP address in CIDR notation");

        if (!int.TryParse(parts[1], out var prefix) || prefix < 0 || prefix > 32)
            return Result<IPAddress>.Failure("Invalid prefix length in CIDR notation");

        return Result<IPAddress>.Success(new IPAddress(trimmedCidr));
    }

    /// <summary>
    /// Checks if this IP address is in IPv4 format
    /// </summary>
    public bool IsIPv4() {
        return System.Net.IPAddress.TryParse(Value, out var addr) &&
               addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
    }

    /// <summary>
    /// Checks if this IP address is in IPv6 format
    /// </summary>
    public bool IsIPv6() {
        return System.Net.IPAddress.TryParse(Value, out var addr) &&
               addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
    }

    protected override IEnumerable<object?> GetEqualityComponents() {
        yield return Value.ToLowerInvariant();
    }

    public override string ToString() => Value;
}