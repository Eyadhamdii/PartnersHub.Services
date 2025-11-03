using PartnersHub.CiamMigrationTool.Models;

namespace PartnersHub.CiamMigrationTool.Services;

public interface IUserMappingService
{
    CiamUser MapToCiamUser(MicrosoftIdentityUser user);
    string GenerateUsername(MicrosoftIdentityUser user);
    string ExtractCompanyId(MicrosoftIdentityUser user);
}

public class UserMappingService : IUserMappingService
{
    private readonly ISimpleLogger _logger;

    public UserMappingService(ISimpleLogger logger)
    {
        _logger = logger;
    }

    public CiamUser MapToCiamUser(MicrosoftIdentityUser user)
    {
        try
        {
            _logger.LogInformation($"Mapping Microsoft user {user.UserPrincipalName} to CIAM format");

            var ciamUser = new CiamUser
            {
                UserName = GenerateUsername(user),
                DisplayName = user.DisplayName,
                Name = new CiamUserName
                {
                    GivenName = user.GivenName,
                    FamilyName = user.Surname
                },
                Emails = new List<CiamEmail>
                {
                    new() { Value = user.Mail, Primary = true }
                },
                EnterpriseExtension = new CiamEnterpriseExtension
                {
                    Organization = string.IsNullOrEmpty(user.CompanyName) ? "PIF" : user.CompanyName,
                    AccountLocked = false,
                    AccountDisabled = !user.AccountEnabled
                },
                Wso2Extension = new CiamWso2Extension
                {
                    AskPassword = "true", // Send invitation email
                    Country = "Saudi Arabia",
                    AccountLocked = false,
                    AccountState = user.AccountEnabled ? "UNLOCKED" : "LOCKED"
                },
                CustomExtension = new CiamCustomExtension
                {
                    CompanyId = ExtractCompanyId(user),
                    Participant = $"Migrated user from ASP.NET Core Identity - {user.DisplayName}"
                }
            };

            // Add phone number if available
            if (!string.IsNullOrEmpty(user.MobilePhone))
            {
                ciamUser.PhoneNumbers.Add(new CiamPhoneNumber 
                { 
                    Value = user.MobilePhone, 
                    Type = "mobile" 
                });
            }

            _logger.LogInformation($"Successfully mapped user {user.UserPrincipalName} to CIAM username {ciamUser.UserName}");

            return ciamUser;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error mapping user {user.UserPrincipalName} to CIAM format: {ex.Message}");
            throw;
        }
    }

    public string GenerateUsername(MicrosoftIdentityUser user)
    {
        try
        {
            // Use the part before @ in email as username, or fall back to displayName
            if (!string.IsNullOrEmpty(user.Mail))
            {
                var emailPart = user.Mail.Split('@')[0];
                return SanitizeUsername(emailPart);
            }

            if (!string.IsNullOrEmpty(user.UserPrincipalName))
            {
                var upnPart = user.UserPrincipalName.Split('@')[0];
                return SanitizeUsername(upnPart);
            }

            // Fallback to display name
            return SanitizeUsername(user.DisplayName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating username for user {user.UserPrincipalName}: {ex.Message}");
            // Fallback to a safe default
            return $"user_{Guid.NewGuid():N}"[..16];
        }
    }

    public string ExtractCompanyId(MicrosoftIdentityUser user)
    {
        try
        {
            // Try to extract company ID from various fields
            // This is business logic - customize as needed
            if (!string.IsNullOrEmpty(user.Department))
            {
                return Math.Abs(user.Department.GetHashCode()).ToString();
            }

            if (!string.IsNullOrEmpty(user.CompanyName))
            {
                return Math.Abs(user.CompanyName.GetHashCode()).ToString();
            }

            if (!string.IsNullOrEmpty(user.JobTitle))
            {
                return Math.Abs(user.JobTitle.GetHashCode()).ToString();
            }

            // Default company ID for PIF
            return "76559421";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error extracting company ID for user {user.UserPrincipalName}: {ex.Message}");
            return "76559421"; // Default fallback
        }
    }

    private string SanitizeUsername(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "defaultuser";

        // Replace spaces and underscores with dots, convert to lowercase
        var sanitized = input
            .Replace(" ", ".")
            .Replace("_", ".")
            .ToLower();

        // Remove any invalid characters (keep only alphanumeric, dots, and hyphens)
        sanitized = new string(sanitized
            .Where(c => char.IsLetterOrDigit(c) || c == '.' || c == '-')
            .ToArray());

        // Ensure it's not empty and not too long
        if (string.IsNullOrEmpty(sanitized))
            sanitized = "user";

        if (sanitized.Length > 64) // CIAM username limit
            sanitized = sanitized[..64];

        return sanitized;
    }
}