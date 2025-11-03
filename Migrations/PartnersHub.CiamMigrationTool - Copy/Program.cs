using System.Text.Json;
using PartnersHub.CiamMigrationTool.Models;
using PartnersHub.CiamMigrationTool.Services;
using Microsoft.Extensions.Configuration;

namespace PartnersHub.CiamMigrationTool;

// Service container class
public class ServiceContainer
{
    public ISimpleLogger Logger { get; set; } = null!;
    public IMigrationService MigrationService { get; set; } = null!;
    public IMicrosoftGraphService GraphService { get; set; } = null!;
    public ICiamService CiamService { get; set; } = null!;
    public IUserMappingService UserMappingService { get; set; } = null!;
    public AppConfiguration Configuration { get; set; } = null!;
}

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Partners Hub CIAM Migration Tool ===");
        Console.WriteLine("Enhanced with Invitation Model & Bulk Operations");
        Console.WriteLine("Starting migration from ASP.NET Core Identity to CIAM...\n");

        try
        {
            // Load configuration from appsettings.json
            var configuration = LoadConfiguration();
            
            // Initialize services
            var serviceContainer = InitializeServices(configuration);
            await ShowMenuAsync(serviceContainer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[ERROR] Critical error: {ex.Message}");
            Console.WriteLine($"Details: {ex}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private static IConfiguration LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        return builder.Build();
    }

    private static ServiceContainer InitializeServices(IConfiguration configuration)
    {
        var logger = new ConsoleLogger();
        
        // Load configurations from appsettings.json
        var jwtConfig = new JwtConfiguration();
        configuration.GetSection("Jwt").Bind(jwtConfig);

        var connectionStrings = new ConnectionStringsConfiguration();
        configuration.GetSection("ConnectionStrings").Bind(connectionStrings);

        // Migration tool specific configurations
        var graphConfig = new MicrosoftGraphConfiguration
        {
            ClientId = "not-used-for-identity-db",
            ClientSecret = "not-used-for-identity-db",
            TenantId = "not-used-for-identity-db"
        };

        var ciamConfig = new CiamConfiguration();
        configuration.GetSection("CIAM").Bind(ciamConfig);

        var migrationConfig = new MigrationConfiguration();
        configuration.GetSection("Migration").Bind(migrationConfig);

        logger.LogInformation("Configuration loaded successfully");
        logger.LogInformation($"JWT Issuer: {jwtConfig.Issuer}");
        logger.LogInformation($"JWT Audience: {jwtConfig.Audience}");
        logger.LogInformation($"CIAM Base URL: {ciamConfig.BaseUrl}");
        logger.LogInformation($"Migration Mode: {(migrationConfig.UseInvitationModel ? "Invitation-based" : "Direct/Bulk")}");
        logger.LogInformation($"Database Connection: {(string.IsNullOrEmpty(connectionStrings.DefaultConnection) ? "Not configured" : "Configured")}");

        // Initialize services
        var httpClient = new HttpClient();
        var migrationRepository = new InMemoryMigrationRepository(logger);
        var userMappingService = new UserMappingService(logger);
        var graphService = new MicrosoftGraphService(httpClient, graphConfig, jwtConfig, connectionStrings, logger);
        var ciamService = new CiamService(httpClient, ciamConfig, logger);
        var migrationService = new MigrationService(
            graphService, 
            ciamService, 
            userMappingService, 
            migrationRepository, 
            migrationConfig, 
            logger);

        return new ServiceContainer
        {
            Logger = logger,
            MigrationService = migrationService,
            GraphService = graphService,
            CiamService = ciamService,
            UserMappingService = userMappingService,
            Configuration = new AppConfiguration
            {
                Jwt = jwtConfig,
                ConnectionStrings = connectionStrings,
                CIAM = ciamConfig,
                Migration = migrationConfig,
                ApiKey = configuration["ApiKey"] ?? string.Empty
            }
        };
    }

    private static async Task ShowMenuAsync(ServiceContainer services)
    {
        while (true)
        {
            Console.WriteLine("\n=== Enhanced CIAM Migration Options ===");
            Console.WriteLine("1. Start Smart Migration (Auto-detect: Bulk or Invitation)");
            Console.WriteLine("2. Start Bulk Migration (SCIM Bulk API)");
            Console.WriteLine("3. Start Invitation-Based Migration");
            Console.WriteLine("4. View Migration Status");
            Console.WriteLine("5. Retry Failed Migrations");
            Console.WriteLine("6. Test ASP.NET Core Identity Connection");
            Console.WriteLine("7. Test CIAM Connection & Features");
            Console.WriteLine("8. Migrate Single User");
            Console.WriteLine("9. User Mapping Test");
            Console.WriteLine("10. JWT Token Test");
            Console.WriteLine("11. Database Connection Test");
            Console.WriteLine("12. Test CIAM Advanced Features");
            Console.WriteLine("13. CIAM Connectivity Diagnostics");
            Console.WriteLine("14. CIAM Credentials Validator & Troubleshooter");
            Console.WriteLine("0. Exit");
            Console.Write("\nSelect an option: ");

            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await StartSmartMigrationAsync(services.MigrationService, services.Configuration.Migration);
                        break;
                    case "2":
                        await StartBulkMigrationAsync(services.MigrationService);
                        break;
                    case "3":
                        await StartInvitationMigrationAsync(services.MigrationService);
                        break;
                    case "4":
                        await ViewMigrationStatusAsync(services.MigrationService);
                        break;
                    case "5":
                        await RetryFailedMigrationsAsync(services.MigrationService);
                        break;
                    case "6":
                        await TestIdentityConnectionAsync(services.GraphService);
                        break;
                    case "7":
                        await TestCiamConnectionAsync(services.CiamService);
                        break;
                    case "8":
                        await MigrateSingleUserAsync(services);
                        break;
                    case "9":
                        await TestUserMappingAsync(services.UserMappingService, services.GraphService);
                        break;
                    case "10":
                        await TestJwtTokenAsync(services.GraphService);
                        break;
                    case "11":
                        await TestDatabaseConnectionAsync(services.GraphService);
                        break;
                    case "12":
                        await TestCiamAdvancedFeaturesAsync(services.CiamService, services.Configuration.CIAM);
                        break;
                    case "13":
                        await TestCiamConnectivityDiagnosticsAsync(services.CiamService, services.Configuration.CIAM);
                        break;
                    case "14":
                        await TestCiamCredentialsValidatorAsync(services.Configuration.CIAM);
                        break;
                    case "0":
                        Console.WriteLine("Goodbye!");
                        services.Logger.LogInformation("Application shutdown requested by user");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] Operation failed: {ex.Message}");
                services.Logger.LogError($"Menu operation failed: {ex.Message}");
            }
        }
    }

    private static async Task StartSmartMigrationAsync(IMigrationService migrationService, MigrationConfiguration config)
    {
        Console.WriteLine($"\n[MIGRATION] Starting smart migration process...");
        Console.WriteLine($"Mode: {(config.UseInvitationModel ? "Invitation-based" : "Bulk/Direct")}");
        Console.WriteLine($"Send Invitation Emails: {config.SendInvitationEmails}");
        Console.WriteLine($"Batch Size: {config.BatchSize}");
        Console.Write("Are you sure you want to continue? (y/N): ");

        if (Console.ReadLine()?.ToLower() != "y")
        {
            Console.WriteLine("Migration cancelled.");
            return;
        }

        var startTime = DateTime.Now;
        var migratedCount = await migrationService.StartMigrationAsync();
        var duration = DateTime.Now - startTime;

        Console.WriteLine($"\n[SUCCESS] Smart migration completed!");
        Console.WriteLine($"  * {(config.UseInvitationModel ? "Invitations sent" : "Users migrated")}: {migratedCount}");
        Console.WriteLine($"  * Duration: {duration:hh\\:mm\\:ss}");
    }

    private static async Task StartBulkMigrationAsync(IMigrationService migrationService)
    {
        Console.WriteLine("\n[BULK] Starting bulk migration using SCIM Bulk API...");
        Console.WriteLine("This will migrate users in batches using the CIAM bulk endpoint.");
        Console.Write("Are you sure you want to continue? (y/N): ");

        if (Console.ReadLine()?.ToLower() != "y")
        {
            Console.WriteLine("Bulk migration cancelled.");
            return;
        }

        var startTime = DateTime.Now;
        var migratedCount = await migrationService.StartBulkMigrationAsync();
        var duration = DateTime.Now - startTime;

        Console.WriteLine($"\n[SUCCESS] Bulk migration completed!");
        Console.WriteLine($"  * Users migrated: {migratedCount}");
        Console.WriteLine($"  * Duration: {duration:hh\\:mm\\:ss}");
    }

    private static async Task StartInvitationMigrationAsync(IMigrationService migrationService)
    {
        Console.WriteLine("\n[INVITATION] Starting invitation-based migration...");
        Console.WriteLine("This will create user invitations and send invitation emails via CIAM.");
        Console.Write("Are you sure you want to continue? (y/N): ");

        if (Console.ReadLine()?.ToLower() != "y")
        {
            Console.WriteLine("Invitation migration cancelled.");
            return;
        }

        var startTime = DateTime.Now;
        var invitedCount = await migrationService.StartInvitationBasedMigrationAsync();
        var duration = DateTime.Now - startTime;

        Console.WriteLine($"\n[SUCCESS] Invitation migration completed!");
        Console.WriteLine($"  * Invitations sent: {invitedCount}");
        Console.WriteLine($"  * Duration: {duration:hh\\:mm\\:ss}");
    }

    private static async Task TestCiamAdvancedFeaturesAsync(ICiamService ciamService, CiamConfiguration config)
    {
        Console.WriteLine("\n[TEST] Testing CIAM Advanced Features...");

        try
        {
            var accessToken = await ciamService.GetAccessTokenAsync();
            
            Console.WriteLine($"[SUCCESS] Access token obtained");
            Console.WriteLine($"  * CIAM Version: {config.Version}");
            Console.WriteLine($"  * Base URL: {config.BaseUrl}");

            // Test features
            var featuresWorking = await ciamService.TestCiamFeaturesAsync(accessToken);
            Console.WriteLine($"\n[FEATURES] CIAM Features Status:");
            Console.WriteLine($"  * Login: {(config.Features.Login ? "[OK]" : "[FAIL]")} Supported");
            Console.WriteLine($"  * Forgot/Reset Password: {(config.Features.ForgotResetPassword ? "[OK]" : "[FAIL]")} Supported");
            Console.WriteLine($"  * Email Verification: {(config.Features.EmailVerification ? "[OK]" : "[FAIL]")} Supported");
            Console.WriteLine($"  * Account Activation: {(config.Features.AccountActivation ? "[OK]" : "[FAIL]")} Supported");
            Console.WriteLine($"  * OTP/2FA: {(config.Features.OtpTwoFactorAuth ? "[OK]" : "[FAIL]")} Supported");
            Console.WriteLine($"  * Session Timeout: {(config.Features.SessionTimeout ? "[OK]" : "[FAIL]")} Supported");
            Console.WriteLine($"  * Configurable Redirects: {(config.Features.ConfigurableRedirects ? "[OK]" : "[FAIL]")} Supported");
            Console.WriteLine($"  * Custom Branding: {(config.Features.CustomBranding ? "[OK]" : "[FAIL]")} Supported");
            Console.WriteLine($"  * Arabic/English Support: {(config.Features.ArabicEnglishSupport ? "[OK]" : "[FAIL]")} Supported");
            Console.WriteLine($"  * RTL Support: {(config.Features.RtlSupport ? "[OK]" : "[FAIL]")} Supported");

            Console.WriteLine($"\n[SCOPES] Available Scopes:");
            var scopes = config.Scopes.Split(' ');
            foreach (var scope in scopes)
            {
                Console.WriteLine($"  * {scope}");
            }

            Console.WriteLine($"\n[CLAIMS] Supported Claims:");
            foreach (var claim in config.SupportedClaims)
            {
                Console.WriteLine($"  * {claim}");
            }

            // Test UserInfo endpoint
            try
            {
                var userInfo = await ciamService.GetUserInfoAsync(accessToken);
                Console.WriteLine($"\n[USERINFO] UserInfo Endpoint: {(userInfo != null ? "[OK] Working" : "[FAIL] Failed")}");
                if (userInfo != null)
                {
                    Console.WriteLine($"  * Subject: {userInfo.Subject}");
                    Console.WriteLine($"  * Email: {userInfo.Email}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[USERINFO] UserInfo Endpoint: [FAIL] Failed - {ex.Message}");
            }

            Console.WriteLine($"\n[STATUS] Overall Status: {(featuresWorking ? "[OK] All systems operational" : "[WARNING] Some issues detected")}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] CIAM advanced features test failed: {ex.Message}");
        }
    }

    private static async Task ViewMigrationStatusAsync(IMigrationService migrationService)
    {
        Console.WriteLine("\n[REPORT] Enhanced Migration Status Report");
        Console.WriteLine("===================================");

        var records = await migrationService.GetMigrationStatusAsync();
        
        if (!records.Any())
        {
            Console.WriteLine("No migration records found.");
            return;
        }

        var statusGroups = records.GroupBy(r => r.Status).ToList();

        Console.WriteLine("\nSummary:");
        foreach (var group in statusGroups)
        {
            var icon = group.Key switch
            {
                MigrationStatus.Completed => "[OK]",
                MigrationStatus.Failed => "[FAIL]",
                MigrationStatus.InProgress => "[PROGRESS]",
                MigrationStatus.Pending => "[PENDING]",
                MigrationStatus.Skipped => "[SKIP]",
                _ => "[UNKNOWN]"
            };
            Console.WriteLine($"  {icon} {group.Key}: {group.Count()}");
        }

        Console.WriteLine($"\nTotal records: {records.Count}");

        // Show recent failed migrations
        var failedRecords = records.Where(r => r.Status == MigrationStatus.Failed).Take(5).ToList();
        if (failedRecords.Any())
        {
            Console.WriteLine("\nRecent Failures:");
            foreach (var record in failedRecords)
            {
                Console.WriteLine($"  * {record.MicrosoftUserPrincipalName}: {record.ErrorMessage}");
            }
        }

        // Show recent successful migrations
        var completedRecords = records.Where(r => r.Status == MigrationStatus.Completed).Take(5).ToList();
        if (completedRecords.Any())
        {
            Console.WriteLine("\nRecent Successes:");
            foreach (var record in completedRecords)
            {
                var successType = record.ErrorMessage?.Contains("Invitation") == true ? "Invitation" : "Direct Migration";
                Console.WriteLine($"  * {record.MicrosoftUserPrincipalName} -> {record.CiamUserName} ({successType})");
            }
        }
    }

    private static async Task RetryFailedMigrationsAsync(IMigrationService migrationService)
    {
        Console.WriteLine("\n[RETRY] Retrying failed migrations...");
        
        var retriedCount = await migrationService.RetryFailedMigrationsAsync();
        
        Console.WriteLine($"[SUCCESS] Retry completed! Successfully retried {retriedCount} migrations.");
    }

    private static async Task TestIdentityConnectionAsync(IMicrosoftGraphService graphService)
    {
        Console.WriteLine("\n[TEST] Testing ASP.NET Core Identity connection...");
        
        try
        {
            var connectionTest = await graphService.TestDatabaseConnectionAsync();
            if (!connectionTest)
            {
                Console.WriteLine("[FAIL] Database connection failed");
                return;
            }

            var totalUsers = await graphService.GetTotalUsersCountAsync();
            Console.WriteLine($"[SUCCESS] Successfully connected to Identity system");
            Console.WriteLine($"  * Total users available: {totalUsers}");

            // Get a sample user
            var sampleUsers = await graphService.GetUsersAsync(0, 1);
            if (sampleUsers.Any())
            {
                var user = sampleUsers.First();
                Console.WriteLine($"  * Sample user: {user.DisplayName} ({user.UserPrincipalName})");
                Console.WriteLine($"  * User ID: {user.Id}");
                Console.WriteLine($"  * Email: {user.Mail}");
                Console.WriteLine($"  * Department: {user.Department}");
                Console.WriteLine($"  * Account Enabled: {user.AccountEnabled}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Identity system connection failed: {ex.Message}");
        }
    }

    private static async Task TestCiamConnectionAsync(ICiamService ciamService)
    {
        Console.WriteLine("\n[TEST] Testing CIAM connection...");
        
        try
        {
            // Test basic connectivity first
            var connectivityTest = await ciamService.TestCiamConnectivityAsync();
            Console.WriteLine($"[CONNECTIVITY] Basic connectivity: {(connectivityTest ? "[OK] Success" : "[FAIL] Failed")}");
            
            if (!connectivityTest)
            {
                Console.WriteLine("[ERROR] Cannot proceed with token test - basic connectivity failed");
                return;
            }

            // Test discovery document
            var discovery = await ciamService.GetDiscoveryDocumentAsync();
            if (discovery != null)
            {
                Console.WriteLine("[DISCOVERY] Discovery document retrieved successfully");
                Console.WriteLine($"  * Issuer: {discovery.Issuer}");
                Console.WriteLine($"  * Token Endpoint: {discovery.TokenEndpoint}");
                Console.WriteLine($"  * UserInfo Endpoint: {discovery.UserinfoEndpoint}");
                Console.WriteLine($"  * Supported Grant Types: {string.Join(", ", discovery.GrantTypesSupported ?? new List<string>())}");
                Console.WriteLine($"  * Supported Scopes: {string.Join(", ", discovery.ScopesSupported ?? new List<string>())}");
            }

            // Test token acquisition
            var accessToken = await ciamService.GetAccessTokenAsync();
            Console.WriteLine($"[SUCCESS] Successfully connected to CIAM");
            Console.WriteLine($"  * Access token obtained (length: {accessToken.Length})");
            Console.WriteLine($"  * Token preview: {accessToken[..Math.Min(50, accessToken.Length)]}...");

            // Test features
            var featuresTest = await ciamService.TestCiamFeaturesAsync(accessToken);
            Console.WriteLine($"  * Features test: {(featuresTest ? "[OK] Passed" : "[WARNING] Issues detected")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] CIAM connection failed: {ex.Message}");
            
            // Provide specific troubleshooting guidance
            Console.WriteLine("\n[TROUBLESHOOTING] Connection failed. Please check:");
            Console.WriteLine("  * Network connectivity to uat-api.pif.gov.sa:9003");
            Console.WriteLine("  * CIAM client credentials (ClientId/ClientSecret)");
            Console.WriteLine("  * Token endpoint URL format");
            Console.WriteLine("  * Firewall/proxy settings");
            Console.WriteLine("  * SSL/TLS certificate trust");
        }
    }

    private static async Task MigrateSingleUserAsync(ServiceContainer services)
    {
        Console.Write("\nEnter user email or User Principal Name: ");
        var userIdentifier = Console.ReadLine();

        if (string.IsNullOrEmpty(userIdentifier))
        {
            Console.WriteLine("Invalid input.");
            return;
        }

        Console.WriteLine($"\n[SEARCH] Looking for user: {userIdentifier}");

        try
        {
            var users = await services.GraphService.SearchUsersByEmailAsync(userIdentifier);
            
            if (!users.Any())
            {
                Console.WriteLine("[FAIL] User not found.");
                return;
            }

            var user = users.First();
            Console.WriteLine($"[SUCCESS] Found user: {user.DisplayName} ({user.UserPrincipalName})");
            Console.WriteLine($"  * User ID: {user.Id}");
            Console.WriteLine($"  * Email: {user.Mail}");
            Console.WriteLine($"  * Department: {user.Department}");
            Console.WriteLine($"  * Account Enabled: {user.AccountEnabled}");
            
            var migrationMode = services.Configuration.Migration.UseInvitationModel ? "Invitation" : "Direct";
            Console.WriteLine($"[MIGRATION] Starting {migrationMode} migration...");

            var result = await services.MigrationService.MigrateUserAsync(user);

            if (result?.Status == MigrationStatus.Completed)
            {
                Console.WriteLine($"[SUCCESS] User migration successful!");
                Console.WriteLine($"  * CIAM User ID: {result.CiamUserId}");
                Console.WriteLine($"  * CIAM Username: {result.CiamUserName}");
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Console.WriteLine($"  * Details: {result.ErrorMessage}");
                }
            }
            else
            {
                Console.WriteLine($"[FAIL] Migration failed: {result?.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Error during single user migration: {ex.Message}");
        }
    }

    private static async Task TestUserMappingAsync(IUserMappingService userMappingService, IMicrosoftGraphService graphService)
    {
        Console.WriteLine("\n[TEST] Testing Enhanced User Mapping (ASP.NET Core Identity -> CIAM)...");

        try
        {
            var users = await graphService.GetUsersAsync(0, 3);
            
            foreach (var user in users)
            {
                Console.WriteLine($"\n[MAPPING] Mapping user: {user.DisplayName}");
                var ciamUser = userMappingService.MapToCiamUser(user);
                
                Console.WriteLine($"  * ASP.NET Core Identity:");
                Console.WriteLine($"    - UPN: {user.UserPrincipalName}");
                Console.WriteLine($"    - Email: {user.Mail}");
                Console.WriteLine($"    - Display Name: {user.DisplayName}");
                Console.WriteLine($"    - Department: {user.Department}");
                Console.WriteLine($"    - Job Title: {user.JobTitle}");
                Console.WriteLine($"    - Enabled: {user.AccountEnabled}");
                
                Console.WriteLine($"  * CIAM Mapping:");
                Console.WriteLine($"    - Username: {ciamUser.UserName}");
                Console.WriteLine($"    - Display Name: {ciamUser.DisplayName}");
                Console.WriteLine($"    - Email: {ciamUser.Emails.FirstOrDefault()?.Value}");
                Console.WriteLine($"    - Phone: {ciamUser.PhoneNumbers.FirstOrDefault()?.Value}");
                Console.WriteLine($"    - Organization: {ciamUser.EnterpriseExtension.Organization}");
                Console.WriteLine($"    - Company ID: {ciamUser.CustomExtension.CompanyId}");
                Console.WriteLine($"    - Account State: {ciamUser.Wso2Extension.AccountState}");
                Console.WriteLine($"    - Ask Password: {ciamUser.Wso2Extension.AskPassword}");
                Console.WriteLine($"    - Participant: {ciamUser.CustomExtension.Participant}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] User mapping test failed: {ex.Message}");
        }
    }

    private static async Task TestJwtTokenAsync(IMicrosoftGraphService graphService)
    {
        Console.WriteLine("\n[SECURITY] Testing JWT Token Generation and Validation...");

        try
        {
            // Generate token
            var token = await graphService.GetAccessTokenAsync();
            Console.WriteLine($"[SUCCESS] JWT Token generated successfully");
            Console.WriteLine($"  * Token length: {token.Length}");
            Console.WriteLine($"  * Token preview: {token[..Math.Min(100, token.Length)]}...");

            // Validate token
            var isValid = await graphService.ValidateTokenAsync(token);
            Console.WriteLine($"  * Token validation: {(isValid ? "[OK] Valid" : "[FAIL] Invalid")}");

            // Parse token claims
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            
            Console.WriteLine($"  * Token details:");
            Console.WriteLine($"    - Issuer: {jsonToken.Issuer}");
            Console.WriteLine($"    - Audience: {string.Join(", ", jsonToken.Audiences)}");
            Console.WriteLine($"    - Expires: {jsonToken.ValidTo:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine($"    - Claims count: {jsonToken.Claims.Count()}");
            
            Console.WriteLine($"  * Claims:");
            foreach (var claim in jsonToken.Claims.Take(5))
            {
                Console.WriteLine($"    - {claim.Type}: {claim.Value}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] JWT token test failed: {ex.Message}");
        }
    }

    private static async Task TestDatabaseConnectionAsync(IMicrosoftGraphService graphService)
    {
        Console.WriteLine("\n[DATABASE] Testing Database Connections...");

        try
        {
            var connectionTest = await graphService.TestDatabaseConnectionAsync();
            
            if (connectionTest)
            {
                Console.WriteLine("[SUCCESS] ASP.NET Core Identity Database: Connected (simulated)");
                
                var userCount = await graphService.GetTotalUsersCountAsync();
                Console.WriteLine($"  * Total users in Identity system: {userCount}");
                
                var sampleUsers = await graphService.GetUsersAsync(0, 3);
                Console.WriteLine($"  * Sample users retrieved: {sampleUsers.Count}");
                
                foreach (var user in sampleUsers.Take(2))
                {
                    Console.WriteLine($"    - {user.DisplayName} ({user.Mail})");
                }
            }
            else
            {
                Console.WriteLine("[FAIL] ASP.NET Core Identity Database: Connection Failed");
                Console.WriteLine("  * Using fallback sample data");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Database connection test failed: {ex.Message}");
        }
    }

    private static async Task TestCiamConnectivityDiagnosticsAsync(ICiamService ciamService, CiamConfiguration config)
    {
        Console.WriteLine("\n[DIAGNOSTICS] CIAM Connectivity Diagnostics");
        Console.WriteLine("===========================================");

        try
        {
            Console.WriteLine("\n[STEP 1] Configuration Validation");
            Console.WriteLine($"  * Base URL: {config.BaseUrl}");
            Console.WriteLine($"  * Token URL: {config.TokenUrl}");
            Console.WriteLine($"  * Client ID: {config.ClientId}");
            Console.WriteLine($"  * Client Secret: {(!string.IsNullOrEmpty(config.ClientSecret) ? "[CONFIGURED]" : "[MISSING]")}");
            Console.WriteLine($"  * Scopes: {config.Scopes}");

            Console.WriteLine("\n[STEP 2] Network Connectivity Test");
            var connectivityTest = await ciamService.TestCiamConnectivityAsync();
            if (!connectivityTest)
            {
                Console.WriteLine("[ERROR] Basic connectivity failed. Check network and firewall settings.");
                return;
            }

            Console.WriteLine("\n[STEP 3] Discovery Document Analysis");
            var discovery = await ciamService.GetDiscoveryDocumentAsync();
            if (discovery != null)
            {
                Console.WriteLine("[SUCCESS] Discovery document retrieved");
                Console.WriteLine($"  * Issuer: {discovery.Issuer}");
                Console.WriteLine($"  * Authorization Endpoint: {discovery.AuthorizationEndpoint}");
                Console.WriteLine($"  * Token Endpoint: {discovery.TokenEndpoint}");
                Console.WriteLine($"  * UserInfo Endpoint: {discovery.UserinfoEndpoint}");
                Console.WriteLine($"  * JWKS URI: {discovery.JwksUri}");
                
                if (discovery.GrantTypesSupported?.Any() == true)
                {
                    Console.WriteLine($"  * Supported Grant Types: {string.Join(", ", discovery.GrantTypesSupported)}");
                }
                
                if (discovery.ScopesSupported?.Any() == true)
                {
                    Console.WriteLine($"  * Supported Scopes: {string.Join(", ", discovery.ScopesSupported)}");
                }

                // Compare configured token URL with discovery document
                if (!string.IsNullOrEmpty(discovery.TokenEndpoint) && 
                    !discovery.TokenEndpoint.Equals(config.TokenUrl, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"[WARNING] Token URL mismatch:");
                    Console.WriteLine($"  - Configured: {config.TokenUrl}");
                    Console.WriteLine($"  - Discovery: {discovery.TokenEndpoint}");
                }
            }

            Console.WriteLine("\n[STEP 4] Token Acquisition Test");
            var accessToken = await ciamService.GetAccessTokenAsync();
            Console.WriteLine("[SUCCESS] Access token obtained successfully");
            Console.WriteLine($"  * Token Length: {accessToken.Length}");
            Console.WriteLine($"  * Token Prefix: {accessToken[..Math.Min(20, accessToken.Length)]}...");

            Console.WriteLine("\n[STEP 5] Token Analysis");
            try
            {
                // Try to decode as JWT (if it's a JWT token)
                if (accessToken.Contains('.'))
                {
                    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    if (handler.CanReadToken(accessToken))
                    {
                        var jwt = handler.ReadJwtToken(accessToken);
                        Console.WriteLine("[INFO] Token is a valid JWT");
                        Console.WriteLine($"  * Issuer: {jwt.Issuer}");
                        Console.WriteLine($"  * Audience: {string.Join(", ", jwt.Audiences)}");
                        Console.WriteLine($"  * Expires: {jwt.ValidTo:yyyy-MM-dd HH:mm:ss} UTC");
                        Console.WriteLine($"  * Claims: {jwt.Claims.Count()}");
                        
                        var scopeClaim = jwt.Claims.FirstOrDefault(c => c.Type == "scope" || c.Type == "scp");
                        if (scopeClaim != null)
                        {
                            Console.WriteLine($"  * Scopes: {scopeClaim.Value}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("[INFO] Token is not a JWT format");
                    }
                }
                else
                {
                    Console.WriteLine("[INFO] Token appears to be an opaque token");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Token analysis failed: {ex.Message}");
            }

            Console.WriteLine("\n[STEP 6] API Endpoint Testing");
            var featuresTest = await ciamService.TestCiamFeaturesAsync(accessToken);
            Console.WriteLine($"[RESULT] Features test: {(featuresTest ? "[OK] Passed" : "[WARNING] Some issues")}");

            Console.WriteLine("\n[DIAGNOSTICS] Overall Status: [SUCCESS] CIAM connectivity is working");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[ERROR] Diagnostics failed: {ex.Message}");
            Console.WriteLine("\n[TROUBLESHOOTING] Common Issues & Solutions:");
            Console.WriteLine("1. Network Connectivity:");
            Console.WriteLine("   - Check if uat-api.pif.gov.sa:9003 is reachable");
            Console.WriteLine("   - Verify firewall/proxy settings");
            Console.WriteLine("   - Test with: telnet uat-api.pif.gov.sa 9003");
            Console.WriteLine("\n2. Authentication Issues:");
            Console.WriteLine("   - Verify Client ID and Client Secret");
            Console.WriteLine("   - Check if client is enabled in CIAM");
            Console.WriteLine("   - Ensure scopes are correctly configured");
            Console.WriteLine("\n3. SSL/TLS Issues:");
            Console.WriteLine("   - Check certificate validity");
            Console.WriteLine("   - Verify TLS version compatibility");
            Console.WriteLine("\n4. URL Configuration:");
            Console.WriteLine("   - Ensure TokenUrl ends with /oauth2/token");
            Console.WriteLine("   - Verify BaseUrl for API endpoints");
        }
    }

    private static async Task TestCiamCredentialsValidatorAsync(CiamConfiguration config)
    {
        Console.WriteLine("\n[VALIDATOR] CIAM Credentials Validator & Troubleshooter");
        Console.WriteLine("======================================================");

        try
        {
            Console.WriteLine("\n[STEP 1] Credentials Configuration Analysis");
            Console.WriteLine("==========================================");
            
            // Validate Client ID format
            Console.WriteLine($"Client ID: {config.ClientId}");
            if (string.IsNullOrEmpty(config.ClientId))
            {
                Console.WriteLine("  [ERROR] Client ID is empty or null");
                return;
            }
            else if (config.ClientId.Length < 10)
            {
                Console.WriteLine("  [WARNING] Client ID seems too short (expected longer alphanumeric string)");
            }
            else if (config.ClientId.Contains(' '))
            {
                Console.WriteLine("  [ERROR] Client ID contains spaces (not valid)");
            }
            else
            {
                Console.WriteLine("  [OK] Client ID format appears valid");
            }

            // Validate Client Secret format  
            Console.WriteLine($"Client Secret: {new string('*', Math.Min(config.ClientSecret.Length, 20))}... (length: {config.ClientSecret.Length})");
            if (string.IsNullOrEmpty(config.ClientSecret))
            {
                Console.WriteLine("  [ERROR] Client Secret is empty or null");
                return;
            }
            else if (config.ClientSecret.Length < 10)
            {
                Console.WriteLine("  [WARNING] Client Secret seems too short");
            }
            else if (config.ClientSecret.Contains(' '))
            {
                Console.WriteLine("  [ERROR] Client Secret contains spaces (not valid)");
            }
            else
            {
                Console.WriteLine("  [OK] Client Secret format appears valid");
            }

            Console.WriteLine("\n[STEP 2] Token Endpoint Validation");
            Console.WriteLine("==================================");
            Console.WriteLine($"Configured Token URL: {config.TokenUrl}");
            
            if (!Uri.IsWellFormedUriString(config.TokenUrl, UriKind.Absolute))
            {
                Console.WriteLine("  [ERROR] Token URL is not a valid absolute URL");
                return;
            }
            
            if (!config.TokenUrl.StartsWith("https://"))
            {
                Console.WriteLine("  [WARNING] Token URL should use HTTPS for security");
            }
            
            Console.WriteLine("  [OK] Token URL format is valid");

            Console.WriteLine("\n[STEP 3] Network Connectivity Test");
            Console.WriteLine("==================================");
            
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);
            
            try
            {
                var discoveryUrl = config.DiscoveryUrl ?? $"{config.TokenUrl}/.well-known/openid-configuration";
                Console.WriteLine($"Testing discovery endpoint: {discoveryUrl}");
                
                var response = await httpClient.GetAsync(discoveryUrl);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("  [OK] Discovery endpoint is reachable");
                }
                else
                {
                    Console.WriteLine($"  [ERROR] Discovery endpoint returned: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] Network connectivity failed: {ex.Message}");
                return;
            }

            Console.WriteLine("\n[STEP 4] Authentication Method Testing");
            Console.WriteLine("=====================================");
            
            // Test Method 1: Client credentials in request body
            Console.WriteLine("Testing Method 1: Client credentials in request body...");
            var method1Result = await TestAuthenticationMethod1(httpClient, config);
            
            if (!method1Result)
            {
                // Test Method 2: Basic authentication
                Console.WriteLine("Testing Method 2: Basic authentication header...");
                var method2Result = await TestAuthenticationMethod2(httpClient, config);
                
                if (!method2Result)
                {
                    Console.WriteLine("\n[STEP 5] Alternative Endpoint Testing");
                    Console.WriteLine("====================================");
                    await TestAlternativeEndpoints(httpClient, config);
                }
            }

            Console.WriteLine("\n[FINAL RECOMMENDATIONS]");
            Console.WriteLine("=======================");
            
            Console.WriteLine("Based on the error 'Client credentials are invalid', please verify:");
            Console.WriteLine("1. [CRITICAL] Contact CIAM administrator to verify:");
            Console.WriteLine("   - Client ID is correctly registered in CIAM");
            Console.WriteLine("   - Client Secret matches the registered value");
            Console.WriteLine("   - Client is enabled and not disabled/expired");
            Console.WriteLine("   - Required scopes are granted to the client");
            Console.WriteLine("");
            Console.WriteLine("2. [VERIFICATION] Double-check configuration:");
            Console.WriteLine("   - No extra spaces in ClientId or ClientSecret");
            Console.WriteLine("   - No special characters got corrupted during copy/paste");
            Console.WriteLine("   - Configuration file was saved and reloaded properly");
            Console.WriteLine("");
            Console.WriteLine("3. [ENVIRONMENT] Confirm you're using the right environment:");
            Console.WriteLine("   - UAT credentials for UAT environment");
            Console.WriteLine("   - Production credentials for Production environment");
            Console.WriteLine("");
            Console.WriteLine("4. [SCOPES] Verify the requested scopes are allowed:");
            foreach (var scope in config.Scopes.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                Console.WriteLine($"   - {scope}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[ERROR] Credentials validation failed: {ex.Message}");
        }
    }

    private static async Task<bool> TestAuthenticationMethod1(HttpClient httpClient, CiamConfiguration config)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, config.TokenUrl);
            var formParams = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "client_credentials"),
                new("client_id", config.ClientId),
                new("client_secret", config.ClientSecret),
                new("scope", config.Scopes)
            };

            request.Content = new FormUrlEncodedContent(formParams);
            request.Headers.Add("Accept", "application/json");

            var response = await httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"  Response Status: {response.StatusCode}");
            Console.WriteLine($"  Response Length: {content.Length} characters");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("  [SUCCESS] Method 1 authentication succeeded!");
                return true;
            }
            else
            {
                Console.WriteLine($"  [FAILED] Method 1 authentication failed");
                Console.WriteLine($"  Error Response: {content}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  [ERROR] Method 1 test failed: {ex.Message}");
            return false;
        }
    }

    private static async Task<bool> TestAuthenticationMethod2(HttpClient httpClient, CiamConfiguration config)
    {
        try
        {
            var authValue = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes($"{config.ClientId}:{config.ClientSecret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, config.TokenUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authValue);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", config.Scopes)
            });

            var response = await httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"  Response Status: {response.StatusCode}");
            Console.WriteLine($"  Response Length: {content.Length} characters");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("  [SUCCESS] Method 2 authentication succeeded!");
                return true;
            }
            else
            {
                Console.WriteLine($"  [FAILED] Method 2 authentication failed");
                Console.WriteLine($"  Error Response: {content}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  [ERROR] Method 2 test failed: {ex.Message}");
            return false;
        }
    }

    private static async Task TestAlternativeEndpoints(HttpClient httpClient, CiamConfiguration config)
    {
        var alternativeEndpoints = new[]
        {
            "https://ciam-uat.pif.gov.sa/oauth2/token",
            "https://uat-api.pif.gov.sa:9003/oauth2/token", 
            "https://uat-api.pif.gov.sa:9003/ciam/oauth2/token",
            $"{config.BaseUrl}/oauth2/token"
        };

        Console.WriteLine("Testing alternative token endpoints...");
        
        foreach (var endpoint in alternativeEndpoints.Where(e => e != config.TokenUrl))
        {
            Console.WriteLine($"\nTrying endpoint: {endpoint}");
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
                var formParams = new List<KeyValuePair<string, string>>
                {
                    new("grant_type", "client_credentials"),
                    new("client_id", config.ClientId),
                    new("client_secret", config.ClientSecret),
                    new("scope", config.Scopes)
                };

                request.Content = new FormUrlEncodedContent(formParams);
                request.Headers.Add("Accept", "application/json");

                var response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"  Status: {response.StatusCode}");
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"  [SUCCESS] Alternative endpoint works! Consider updating TokenUrl to: {endpoint}");
                }
                else
                {
                    Console.WriteLine($"  [FAILED] {content}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] {ex.Message}");
            }
        }
    }
}
