using PartnersHub.Synergy.Domain.Common;
using System.Text.RegularExpressions;

namespace PartnersHub.Synergy.Domain.ValueObjects;

/// <summary>
/// Represents company representative contact information
/// </summary>
public class RepresentativeInformation : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PhoneRegex = new(
        @"^[\d\s\-\+\(\)]+$",
        RegexOptions.Compiled);

    public string Name { get; private set; }
    public string Position { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }

    // EF Core constructor
    private RepresentativeInformation()
    {
        Name = string.Empty;
        Position = string.Empty;
        Email = string.Empty;
        Phone = string.Empty;
    }

    private RepresentativeInformation(string name, string position, string email, string phone)
    {
        Name = name;
        Position = position;
        Email = email;
        Phone = phone;
    }

    public static Result<RepresentativeInformation> Create(
        string name,
        string position,
        string email,
        string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<RepresentativeInformation>.Failure("Representative name is required");
        }

        if (string.IsNullOrWhiteSpace(position))
        {
            return Result<RepresentativeInformation>.Failure("Representative position is required");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<RepresentativeInformation>.Failure("Representative email is required");
        }

        if (!EmailRegex.IsMatch(email))
        {
            return Result<RepresentativeInformation>.Failure("Invalid email format");
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            return Result<RepresentativeInformation>.Failure("Representative phone is required");
        }

        if (!PhoneRegex.IsMatch(phone))
        {
            return Result<RepresentativeInformation>.Failure("Invalid phone format");
        }

        return Result<RepresentativeInformation>.Success(new RepresentativeInformation(
            name.Trim(),
            position.Trim(),
            email.Trim().ToLowerInvariant(),
            phone.Trim()));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Email.ToLowerInvariant(); // Email is the unique identifier
    }

    public override string ToString() => $"{Name} ({Position}) - {Email}";
}
