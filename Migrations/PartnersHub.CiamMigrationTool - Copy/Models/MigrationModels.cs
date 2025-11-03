namespace PartnersHub.CiamMigrationTool.Models;

public class MicrosoftIdentityUser
{
    public string Id { get; set; } = string.Empty;
    public string UserPrincipalName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string MobilePhone { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public bool AccountEnabled { get; set; }
    public DateTime CreatedDateTime { get; set; }
}

public class CiamUser
{
    public List<string> Schemas { get; set; } = new()
    {
        "urn:ietf:params:scim:schemas:core:2.0:User",
        "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User",
        "urn:scim:wso2:schema",
        "urn:scim:schemas:extension:custom:User"
    };
    
    public CiamUserName Name { get; set; } = new();
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<CiamEmail> Emails { get; set; } = new();
    public List<CiamPhoneNumber> PhoneNumbers { get; set; } = new();
    
    public CiamEnterpriseExtension EnterpriseExtension { get; set; } = new();
    public CiamWso2Extension Wso2Extension { get; set; } = new();
    public CiamCustomExtension CustomExtension { get; set; } = new();
}

public class CiamUserName
{
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
}

public class CiamEmail
{
    public string Value { get; set; } = string.Empty;
    public bool Primary { get; set; } = true;
}

public class CiamPhoneNumber
{
    public string Value { get; set; } = string.Empty;
    public string Type { get; set; } = "mobile";
}

public class CiamEnterpriseExtension
{
    public string Organization { get; set; } = "PIF";
    public bool AccountLocked { get; set; } = false;
    public bool AccountDisabled { get; set; } = false;
}

public class CiamWso2Extension
{
    public string AskPassword { get; set; } = "true";
    public string Country { get; set; } = "Saudi Arabia";
    public bool AccountLocked { get; set; } = false;
    public string AccountState { get; set; } = "UNLOCKED";
}

public class CiamCustomExtension
{
    public string CompanyId { get; set; } = string.Empty;
    public string Participant { get; set; } = string.Empty;
}

public class CiamUserResponse
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<string> Schemas { get; set; } = new();
    public CiamUserName Name { get; set; } = new();
    public string DisplayName { get; set; } = string.Empty;
    public List<CiamEmail> Emails { get; set; } = new();
    public List<CiamPhoneNumber> PhoneNumbers { get; set; } = new();
}

public class CiamTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string Scope { get; set; } = string.Empty;
}

public class CiamSearchResponse
{
    public List<CiamUserResponse>? Resources { get; set; }
    public int TotalResults { get; set; }
}

public class MigrationRecord
{
    public int Id { get; set; }
    public string MicrosoftUserId { get; set; } = string.Empty;
    public string MicrosoftUserPrincipalName { get; set; } = string.Empty;
    public string? CiamUserId { get; set; }
    public string? CiamUserName { get; set; }
    public MigrationStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? MigratedAt { get; set; }
    public int RetryCount { get; set; }
}

public enum MigrationStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 3,
    Skipped = 4
}