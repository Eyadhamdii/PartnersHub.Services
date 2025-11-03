using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PartnersHub.CiamMigrationTool.Models;
using PartnersHub.CiamMigrationTool.Services;

namespace PartnersHub.CiamMigrationTool;

// Service container class
public class ServiceContainer {
    public ISimpleLogger Logger { get; set; } = null!;
    public IMigrationService MigrationService { get; set; } = null!;
    public IMicrosoftGraphService GraphService { get; set; } = null!;
    public ICiamService CiamService { get; set; } = null!;
    public IUserMappingService UserMappingService { get; set; } = null!;
    public AppConfiguration Configuration { get; set; } = null!;
}

class Program {
    static async Task Main(string[] args) {
        Console.WriteLine("=== Partners Hub CIAM Migration Tool ===");
        Console.WriteLine("Enhanced with Invitation Model & Bulk Operations");
        Console.WriteLine("Starting migration from ASP.NET Core Identity to CIAM...\n");

        try {
            // Load configuration from appsettings.json
            var configuration = LoadConfiguration();

            // Initialize services
            var serviceContainer = InitializeServices(configuration);
            await ShowMenuAsync(serviceContainer);
        } catch (Exception ex) {
            Console.WriteLine($"\n[ERROR] Critical error: {ex.Message}");
            Console.WriteLine($"Details: {ex}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private static IConfiguration LoadConfiguration() {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        return builder.Build();
    }

    private static ServiceContainer InitializeServices(IConfiguration configuration) {
        var logger = new ConsoleLogger();

        // Load configurations from appsettings.json
        var jwtConfig = new JwtConfiguration();
        configuration.GetSection("Jwt").Bind(jwtConfig);

        var connectionStrings = new ConnectionStringsConfiguration();
        configuration.GetSection("ConnectionStrings").Bind(connectionStrings);

        // Migration tool specific configurations
        var graphConfig = new MicrosoftGraphConfiguration {
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

        return new ServiceContainer {
            Logger = logger,
            MigrationService = migrationService,
            GraphService = graphService,
            CiamService = ciamService,
            UserMappingService = userMappingService,
            Configuration = new AppConfiguration {
                Jwt = jwtConfig,
                ConnectionStrings = connectionStrings,
                CIAM = ciamConfig,
                Migration = migrationConfig,
                ApiKey = configuration["ApiKey"] ?? string.Empty
            }
        };
    }

    private static async Task ShowMenuAsync(ServiceContainer services) {
        while (true) {
            Console.WriteLine("\n=== CIAM Migration Options ===");
            Console.WriteLine("1. Start Smart Migration (Auto-detect: Bulk or Invitation)");
            Console.WriteLine("2. Start Bulk Migration (SCIM Bulk API)");
            Console.WriteLine("3. Start Invitation-Based Migration");
            Console.WriteLine("4. View Migration Status");
            Console.WriteLine("5. Retry Failed Migrations");
            Console.WriteLine("6. Migrate Single User");
            Console.WriteLine("7. Test CIAM Connection");
            Console.WriteLine("0. Exit");
            Console.Write("\nSelect an option: ");

            var choice = Console.ReadLine();

            try {
                switch (choice) {
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
                        await MigrateSingleUserAsync(services);
                        break;
                    case "7":
                        await TestCiamConnectionAsync(services.CiamService);
                        break;
                    case "0":
                        Console.WriteLine("Goodbye!");
                        services.Logger.LogInformation("Application shutdown requested by user");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            } catch (Exception ex) {
                Console.WriteLine($"\n[ERROR] Operation failed: {ex.Message}");
                services.Logger.LogError($"Menu operation failed: {ex.Message}");
            }
        }
    }

    private static async Task StartSmartMigrationAsync(IMigrationService migrationService, MigrationConfiguration config) {
        Console.WriteLine($"\n[MIGRATION] Starting smart migration process...");
        Console.WriteLine($"Mode: {(config.UseInvitationModel ? "Invitation-based" : "Bulk/Direct")}");
        Console.WriteLine($"Send Invitation Emails: {config.SendInvitationEmails}");
        Console.WriteLine($"Batch Size: {config.BatchSize}");
        Console.Write("Are you sure you want to continue? (y/N): ");

        if (Console.ReadLine()?.ToLower() != "y") {
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

    private static async Task StartBulkMigrationAsync(IMigrationService migrationService) {
        Console.WriteLine("\n[BULK] Starting bulk migration using SCIM Bulk API...");
        Console.WriteLine("This will migrate users in batches using the CIAM bulk endpoint.");
        Console.Write("Are you sure you want to continue? (y/N): ");

        if (Console.ReadLine()?.ToLower() != "y") {
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

    private static async Task StartInvitationMigrationAsync(IMigrationService migrationService) {
        Console.WriteLine("\n[INVITATION] Starting invitation-based migration...");
        Console.WriteLine("This will create user invitations and send invitation emails via CIAM.");
        Console.Write("Are you sure you want to continue? (y/N): ");

        if (Console.ReadLine()?.ToLower() != "y") {
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

    private static async Task ViewMigrationStatusAsync(IMigrationService migrationService) {
        Console.WriteLine("\n[REPORT] Migration Status Report");
        Console.WriteLine("===========================");

        var records = await migrationService.GetMigrationStatusAsync();

        if (!records.Any()) {
            Console.WriteLine("No migration records found.");
            return;
        }

        var statusGroups = records.GroupBy(r => r.Status).ToList();

        Console.WriteLine("\nSummary:");
        foreach (var group in statusGroups) {
            var icon = group.Key switch {
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
        if (failedRecords.Any()) {
            Console.WriteLine("\nRecent Failures:");
            foreach (var record in failedRecords) {
                Console.WriteLine($"  * {record.MicrosoftUserPrincipalName}: {record.ErrorMessage}");
            }
        }

        // Show recent successful migrations
        var completedRecords = records.Where(r => r.Status == MigrationStatus.Completed).Take(5).ToList();
        if (completedRecords.Any()) {
            Console.WriteLine("\nRecent Successes:");
            foreach (var record in completedRecords) {
                var successType = record.ErrorMessage?.Contains("Invitation") == true ? "Invitation" : "Direct Migration";
                Console.WriteLine($"  * {record.MicrosoftUserPrincipalName} -> {record.CiamUserName} ({successType})");
            }
        }
    }

    private static async Task RetryFailedMigrationsAsync(IMigrationService migrationService) {
        Console.WriteLine("\n[RETRY] Retrying failed migrations...");

        var retriedCount = await migrationService.RetryFailedMigrationsAsync();

        Console.WriteLine($"[SUCCESS] Retry completed! Successfully retried {retriedCount} migrations.");
    }

    private static async Task MigrateSingleUserAsync(ServiceContainer services) {
        Console.Write("\nEnter user email or User Principal Name: ");
        var userIdentifier = Console.ReadLine();

        if (string.IsNullOrEmpty(userIdentifier)) {
            Console.WriteLine("Invalid input.");
            return;
        }

        Console.WriteLine($"\n[SEARCH] Looking for user: {userIdentifier}");

        try {
            var users = await services.GraphService.SearchUsersByEmailAsync(userIdentifier);

            if (!users.Any()) {
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

            if (result?.Status == MigrationStatus.Completed) {
                Console.WriteLine($"[SUCCESS] User migration successful!");
                Console.WriteLine($"  * CIAM User ID: {result.CiamUserId}");
                Console.WriteLine($"  * CIAM Username: {result.CiamUserName}");
                if (!string.IsNullOrEmpty(result.ErrorMessage)) {
                    Console.WriteLine($"  * Details: {result.ErrorMessage}");
                }
            } else {
                Console.WriteLine($"[FAIL] Migration failed: {result?.ErrorMessage}");
            }
        } catch (Exception ex) {
            Console.WriteLine($"[ERROR] Error during single user migration: {ex.Message}");
        }
    }

    private static async Task TestCiamConnectionAsync(ICiamService ciamService) {
        Console.WriteLine("\n[TEST] Testing CIAM connection...");

        try {
            var accessToken = await ciamService.GetAccessTokenAsync();
            Console.WriteLine($"[SUCCESS] Successfully connected to CIAM");
            Console.WriteLine($"  * Access token obtained (length: {accessToken.Length})");
            Console.WriteLine($"  * Token preview: {accessToken[..Math.Min(50, accessToken.Length)]}...");

            // Test a simple user search to verify SCIM access
            Console.WriteLine("\n[TEST] Testing SCIM API access...");
            var testUser = await ciamService.GetUserByEmailAsync("test@example.com", accessToken);
            Console.WriteLine($"  * SCIM API access: [OK] Working (no user found is expected)");
        } catch (Exception ex) {
            Console.WriteLine($"[ERROR] CIAM connection failed: {ex.Message}");

            Console.WriteLine("\n[TROUBLESHOOTING] Please check:");
            Console.WriteLine("  * CIAM client credentials (ClientId/ClientSecret)");
            Console.WriteLine("  * Network connectivity to CIAM server");
            Console.WriteLine("  * Scopes configuration for SCIM access");
            Console.WriteLine("  * BaseUrl points to correct SCIM endpoint");
        }
    }
}