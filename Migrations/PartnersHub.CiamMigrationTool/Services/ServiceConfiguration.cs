namespace PartnersHub.CiamMigrationTool.Services;

// Configuration classes
public class MicrosoftGraphConfiguration {
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
}

public class CiamConfiguration {
    public string BaseUrl { get; set; } = string.Empty;
    public string TokenUrl { get; set; } = string.Empty;
    public string DiscoveryUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string UserAuthenticationScopes { get; set; } = string.Empty;
    public string AdminManagementScopes { get; set; } = string.Empty;
    public string Scopes { get; set; } = string.Empty;
    public string BulkApiEndpoint { get; set; } = "/Users/bulk";
    public string InvitationEndpoint { get; set; } = "/invitations";
    public string UserInfoEndpoint { get; set; } = "/oauth2/userinfo";
    public string IntrospectEndpoint { get; set; } = "/oauth2/introspect";
    public CiamFeatures Features { get; set; } = new();
    public List<string> SupportedClaims { get; set; } = new();
    public string Version { get; set; } = "7.1.0";
}

public class CiamFeatures {
    public bool Login { get; set; } = true;
    public bool ForgotResetPassword { get; set; } = true;
    public bool EmailVerification { get; set; } = true;
    public bool AccountActivation { get; set; } = true;
    public bool OtpTwoFactorAuth { get; set; } = true;
    public bool SessionTimeout { get; set; } = true;
    public bool ConfigurableRedirects { get; set; } = true;
    public bool CustomBranding { get; set; } = true;
    public bool ArabicEnglishSupport { get; set; } = true;
    public bool RtlSupport { get; set; } = true;
}

public class MigrationConfiguration {
    public int BatchSize { get; set; } = 10;
    public int DelayBetweenBatches { get; set; } = 5000;
    public int MaxRetries { get; set; } = 3;
    public bool UseInvitationModel { get; set; } = true;
    public bool SendInvitationEmails { get; set; } = true;
    public bool MigratePasswords { get; set; } = false;
    public List<string> RequiredAttributes { get; set; } = new();
    public List<string> OptionalAttributes { get; set; } = new();
}

// JWT Configuration matching your appsettings.json
public class JwtConfiguration {
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpireMinutes { get; set; } = 30;
    public string PublicKey { get; set; } = string.Empty;
    public string PrivateKey { get; set; } = string.Empty;
}

// Connection Strings Configuration matching your appsettings.json
public class ConnectionStringsConfiguration {
    public string DefaultConnection { get; set; } = string.Empty;
}

// Serilog Configuration (for reference)
public class SerilogConfiguration {
    public List<string> Using { get; set; } = new();
    public SerilogMinimumLevel MinimumLevel { get; set; } = new();
    public List<SerilogWriteTo> WriteTo { get; set; } = new();
    public List<string> Enrich { get; set; } = new();
    public Dictionary<string, string> Properties { get; set; } = new();
}

public class SerilogMinimumLevel {
    public string Default { get; set; } = "Information";
    public Dictionary<string, string> Override { get; set; } = new();
}

public class SerilogWriteTo {
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, object>? Args { get; set; }
}

// Application Configuration Root matching your appsettings.json
public class AppConfiguration {
    public SerilogConfiguration Serilog { get; set; } = new();
    public ConnectionStringsConfiguration ConnectionStrings { get; set; } = new();
    public JwtConfiguration Jwt { get; set; } = new();
    public CiamConfiguration CIAM { get; set; } = new();
    public MigrationConfiguration Migration { get; set; } = new();
    public LoggingConfiguration Logging { get; set; } = new();
    public string ApiKey { get; set; } = string.Empty;
    public string AllowedHosts { get; set; } = string.Empty;
}

public class LoggingConfiguration {
    public Dictionary<string, string> LogLevel { get; set; } = new();
}

// Simple logging interface for demonstration (keeping for compatibility)
public interface ISimpleLogger {
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message);
}

public class ConsoleLogger : ISimpleLogger {
    public void LogInformation(string message) {
        Console.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
    }

    public void LogWarning(string message) {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[WARN] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        Console.ResetColor();
    }

    public void LogError(string message) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        Console.ResetColor();
    }
}