using PartnersHub.CiamMigrationTool.Models;

namespace PartnersHub.CiamMigrationTool.Services;

public interface IMigrationService {
    Task<int> StartMigrationAsync();
    Task<MigrationRecord?> MigrateUserAsync(MicrosoftIdentityUser user);
    Task<List<MigrationRecord>> GetMigrationStatusAsync();
    Task<int> RetryFailedMigrationsAsync();
    Task<int> StartBulkMigrationAsync();
    Task<int> StartInvitationBasedMigrationAsync();
    Task<MigrationRecord?> CreateInvitationForUserAsync(MicrosoftIdentityUser user);
}

public class MigrationService : IMigrationService {
    private readonly IMicrosoftGraphService _graphService;
    private readonly ICiamService _ciamService;
    private readonly IUserMappingService _userMappingService;
    private readonly IMigrationRepository _migrationRepository;
    private readonly MigrationConfiguration _configuration;
    private readonly ISimpleLogger _logger;

    public MigrationService(
        IMicrosoftGraphService graphService,
        ICiamService ciamService,
        IUserMappingService userMappingService,
        IMigrationRepository migrationRepository,
        MigrationConfiguration configuration,
        ISimpleLogger logger) {
        _graphService = graphService;
        _ciamService = ciamService;
        _userMappingService = userMappingService;
        _migrationRepository = migrationRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<int> StartMigrationAsync() {
        if (_configuration.UseInvitationModel) {
            _logger.LogInformation("Starting invitation-based migration as configured");
            return await StartInvitationBasedMigrationAsync();
        } else {
            _logger.LogInformation("Starting bulk migration as configured");
            return await StartBulkMigrationAsync();
        }
    }

    public async Task<int> StartBulkMigrationAsync() {
        try {
            _logger.LogInformation("Starting bulk user migration process using SCIM Bulk API");

            var totalUsers = await _graphService.GetTotalUsersCountAsync();
            _logger.LogInformation($"Total users to migrate in bulk: {totalUsers}");

            var migratedCount = 0;
            var skip = 0;

            // Get access token
            var accessToken = await _ciamService.GetAccessTokenAsync();

            while (skip < totalUsers) {
                _logger.LogInformation($"Processing bulk batch {(skip / _configuration.BatchSize) + 1}: users {skip} to {Math.Min(skip + _configuration.BatchSize, totalUsers)}");

                var users = await _graphService.GetUsersAsync(skip, _configuration.BatchSize);

                if (!users.Any())
                    break;

                // Convert to CIAM users
                var ciamUsers = new List<CiamUser>();
                var migrationRecords = new List<MigrationRecord>();

                foreach (var user in users) {
                    try {
                        // Check if user is already migrated
                        var existingRecord = await _migrationRepository.GetByMicrosoftUserIdAsync(user.Id);
                        if (existingRecord?.Status == MigrationStatus.Completed) {
                            _logger.LogInformation($"User {user.UserPrincipalName} already migrated, skipping");
                            continue;
                        }

                        // Validate required attributes
                        if (!ValidateRequiredAttributes(user)) {
                            _logger.LogWarning($"User {user.UserPrincipalName} missing required attributes, skipping");
                            continue;
                        }

                        var ciamUser = _userMappingService.MapToCiamUser(user);
                        ciamUsers.Add(ciamUser);

                        // Create migration record
                        var migrationRecord = new MigrationRecord {
                            MicrosoftUserId = user.Id,
                            MicrosoftUserPrincipalName = user.UserPrincipalName,
                            Status = MigrationStatus.InProgress,
                            CreatedAt = DateTime.UtcNow,
                            RetryCount = 0
                        };

                        migrationRecords.Add(migrationRecord);
                        await _migrationRepository.AddAsync(migrationRecord);
                    } catch (Exception ex) {
                        _logger.LogError($"Error preparing user {user.UserPrincipalName} for bulk migration: {ex.Message}");
                    }
                }

                // Perform bulk creation
                if (ciamUsers.Any()) {
                    try {
                        var bulkResponse = await _ciamService.CreateUsersBulkAsync(ciamUsers, accessToken);

                        if (bulkResponse?.Operations != null) {
                            // Update migration records based on bulk response
                            for (int i = 0; i < Math.Min(bulkResponse.Operations.Count, migrationRecords.Count); i++) {
                                var operation = bulkResponse.Operations[i];
                                var record = migrationRecords[i];

                                if (operation.Status.StartsWith("2")) // Success (200, 201, etc.)
                                {
                                    record.Status = MigrationStatus.Completed;
                                    record.MigratedAt = DateTime.UtcNow;
                                    record.CiamUserId = ExtractUserIdFromLocation(operation.Location);
                                    record.CiamUserName = ciamUsers[i].UserName;
                                    migratedCount++;
                                } else {
                                    record.Status = MigrationStatus.Failed;
                                    record.ErrorMessage = $"Bulk operation failed with status: {operation.Status}";
                                    record.RetryCount++;
                                }

                                await _migrationRepository.UpdateAsync(record);
                            }
                        }

                        _logger.LogInformation($"Bulk operation completed for {ciamUsers.Count} users");
                    } catch (Exception ex) {
                        _logger.LogError($"Bulk migration failed for batch: {ex.Message}");

                        // Mark all records in this batch as failed
                        foreach (var record in migrationRecords) {
                            record.Status = MigrationStatus.Failed;
                            record.ErrorMessage = ex.Message;
                            record.RetryCount++;
                            await _migrationRepository.UpdateAsync(record);
                        }
                    }
                }

                skip += _configuration.BatchSize;

                // Delay between batches
                if (skip < totalUsers) {
                    _logger.LogInformation($"Waiting {_configuration.DelayBetweenBatches}ms before next batch");
                    await Task.Delay(_configuration.DelayBetweenBatches);
                }
            }

            _logger.LogInformation($"Bulk migration process completed. Successfully migrated {migratedCount} users");
            return migratedCount;
        } catch (Exception ex) {
            _logger.LogError($"Error during bulk migration process: {ex.Message}");
            throw;
        }
    }

    public async Task<int> StartInvitationBasedMigrationAsync() {
        try {
            _logger.LogInformation("Starting invitation-based user migration process");

            var totalUsers = await _graphService.GetTotalUsersCountAsync();
            _logger.LogInformation($"Total users to invite: {totalUsers}");

            var invitedCount = 0;
            var skip = 0;

            while (skip < totalUsers) {
                _logger.LogInformation($"Processing invitation batch {(skip / _configuration.BatchSize) + 1}: users {skip} to {Math.Min(skip + _configuration.BatchSize, totalUsers)}");

                var users = await _graphService.GetUsersAsync(skip, _configuration.BatchSize);

                if (!users.Any())
                    break;

                foreach (var user in users) {
                    try {
                        var result = await CreateInvitationForUserAsync(user);
                        if (result?.Status == MigrationStatus.Completed) {
                            invitedCount++;
                        }

                        // Small delay between individual invitations
                        await Task.Delay(1000);
                    } catch (Exception ex) {
                        _logger.LogError($"Error creating invitation for user {user.UserPrincipalName}: {ex.Message}");
                    }
                }

                skip += _configuration.BatchSize;

                // Delay between batches
                if (skip < totalUsers) {
                    _logger.LogInformation($"Waiting {_configuration.DelayBetweenBatches}ms before next batch");
                    await Task.Delay(_configuration.DelayBetweenBatches);
                }
            }

            _logger.LogInformation($"Invitation-based migration completed. Successfully sent {invitedCount} invitations");
            return invitedCount;
        } catch (Exception ex) {
            _logger.LogError($"Error during invitation-based migration: {ex.Message}");
            throw;
        }
    }

    public async Task<MigrationRecord?> CreateInvitationForUserAsync(MicrosoftIdentityUser user) {
        MigrationRecord? migrationRecord = null;

        try {
            _logger.LogInformation($"Creating invitation for user {user.UserPrincipalName} ({user.Id})");

            // Validate required attributes
            if (!ValidateRequiredAttributes(user)) {
                _logger.LogWarning($"User {user.UserPrincipalName} missing required attributes");
                return null;
            }

            // Get or create migration record
            migrationRecord = await _migrationRepository.GetByMicrosoftUserIdAsync(user.Id);

            if (migrationRecord == null) {
                migrationRecord = new MigrationRecord {
                    MicrosoftUserId = user.Id,
                    MicrosoftUserPrincipalName = user.UserPrincipalName,
                    Status = MigrationStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    RetryCount = 0
                };
                await _migrationRepository.AddAsync(migrationRecord);
            }

            if (migrationRecord.Status == MigrationStatus.Completed) {
                _logger.LogInformation($"User {user.UserPrincipalName} already migrated");
                return migrationRecord;
            }

            // Update status to in progress
            migrationRecord.Status = MigrationStatus.InProgress;
            await _migrationRepository.UpdateAsync(migrationRecord);

            // Get access token
            var accessToken = await _ciamService.GetAccessTokenAsync();

            // Check if user already exists in CIAM
            var existingUser = await _ciamService.GetUserByEmailAsync(user.Mail, accessToken);
            if (existingUser != null) {
                _logger.LogInformation($"User {user.UserPrincipalName} already exists in CIAM with ID {existingUser.Id}");

                migrationRecord.CiamUserId = existingUser.Id;
                migrationRecord.CiamUserName = existingUser.UserName;
                migrationRecord.Status = MigrationStatus.Completed;
                migrationRecord.MigratedAt = DateTime.UtcNow;
                await _migrationRepository.UpdateAsync(migrationRecord);
                return migrationRecord;
            }

            if (_configuration.SendInvitationEmails) {
                // Create CIAM user using proper mapping (this will use correct username format)
                var ciamUser = _userMappingService.MapToCiamUser(user);
                var createdUser = await _ciamService.CreateUserAsync(ciamUser, accessToken);

                if (createdUser != null) {
                    migrationRecord.CiamUserId = createdUser.Id;
                    migrationRecord.CiamUserName = createdUser.UserName;
                    migrationRecord.Status = MigrationStatus.Completed;
                    migrationRecord.MigratedAt = DateTime.UtcNow;
                    migrationRecord.ErrorMessage = "User created with invitation email (askPassword=true)";

                    _logger.LogInformation($"Successfully created invited user {user.UserPrincipalName} in CIAM with ID {createdUser.Id}");
                } else {
                    throw new InvalidOperationException("Failed to create invited user - no response received");
                }
            } else {
                // Create user without sending invitation email
                var ciamUser = _userMappingService.MapToCiamUser(user);
                // Override askPassword to false since we don't want invitation emails
                ciamUser.Wso2Extension.AskPassword = "false";

                var createdUser = await _ciamService.CreateUserAsync(ciamUser, accessToken);

                if (createdUser != null) {
                    migrationRecord.CiamUserId = createdUser.Id;
                    migrationRecord.CiamUserName = createdUser.UserName;
                    migrationRecord.Status = MigrationStatus.Completed;
                    migrationRecord.MigratedAt = DateTime.UtcNow;
                    migrationRecord.ErrorMessage = "User created without invitation email";

                    _logger.LogInformation($"Successfully created user {user.UserPrincipalName} in CIAM with ID {createdUser.Id}");
                } else {
                    throw new InvalidOperationException("Failed to create user - no response received");
                }
            }

            await _migrationRepository.UpdateAsync(migrationRecord);
            return migrationRecord;
        } catch (Exception ex) {
            _logger.LogError($"Failed to create invitation for user {user.UserPrincipalName}: {ex.Message}");

            if (migrationRecord != null) {
                migrationRecord.Status = MigrationStatus.Failed;
                migrationRecord.ErrorMessage = ex.Message;
                migrationRecord.RetryCount++;
                await _migrationRepository.UpdateAsync(migrationRecord);
            }

            return migrationRecord;
        }
    }

    public async Task<MigrationRecord?> MigrateUserAsync(MicrosoftIdentityUser user) {
        if (_configuration.UseInvitationModel) {
            return await CreateInvitationForUserAsync(user);
        } else {
            return await MigrateUserDirectAsync(user);
        }
    }

    private async Task<MigrationRecord?> MigrateUserDirectAsync(MicrosoftIdentityUser user) {
        MigrationRecord? migrationRecord = null;

        try {
            _logger.LogInformation($"Starting direct migration for user {user.UserPrincipalName} ({user.Id})");

            // Get or create migration record
            migrationRecord = await _migrationRepository.GetByMicrosoftUserIdAsync(user.Id);

            if (migrationRecord == null) {
                migrationRecord = new MigrationRecord {
                    MicrosoftUserId = user.Id,
                    MicrosoftUserPrincipalName = user.UserPrincipalName,
                    Status = MigrationStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    RetryCount = 0
                };
                await _migrationRepository.AddAsync(migrationRecord);
            }

            // Update status to in progress
            migrationRecord.Status = MigrationStatus.InProgress;
            await _migrationRepository.UpdateAsync(migrationRecord);

            // Get access token
            var accessToken = await _ciamService.GetAccessTokenAsync();

            // Check if user already exists in CIAM
            var existingUser = await _ciamService.GetUserByEmailAsync(user.Mail, accessToken);
            if (existingUser != null) {
                _logger.LogInformation($"User {user.UserPrincipalName} already exists in CIAM with ID {existingUser.Id}");

                migrationRecord.CiamUserId = existingUser.Id;
                migrationRecord.CiamUserName = existingUser.UserName;
                migrationRecord.Status = MigrationStatus.Completed;
                migrationRecord.MigratedAt = DateTime.UtcNow;
                await _migrationRepository.UpdateAsync(migrationRecord);
                return migrationRecord;
            }

            // Create CIAM user
            var ciamUser = _userMappingService.MapToCiamUser(user);
            var createdUser = await _ciamService.CreateUserAsync(ciamUser, accessToken);

            if (createdUser == null) {
                throw new InvalidOperationException("Failed to create user in CIAM - no response received");
            }

            // Update migration record
            migrationRecord.CiamUserId = createdUser.Id;
            migrationRecord.CiamUserName = createdUser.UserName;
            migrationRecord.Status = MigrationStatus.Completed;
            migrationRecord.MigratedAt = DateTime.UtcNow;
            migrationRecord.ErrorMessage = null;

            await _migrationRepository.UpdateAsync(migrationRecord);

            _logger.LogInformation($"Successfully migrated user {user.UserPrincipalName} to CIAM with ID {createdUser.Id}");

            return migrationRecord;
        } catch (Exception ex) {
            _logger.LogError($"Failed to migrate user {user.UserPrincipalName}: {ex.Message}");

            if (migrationRecord != null) {
                migrationRecord.Status = MigrationStatus.Failed;
                migrationRecord.ErrorMessage = ex.Message;
                migrationRecord.RetryCount++;
                await _migrationRepository.UpdateAsync(migrationRecord);
            }

            return migrationRecord;
        }
    }

    public async Task<List<MigrationRecord>> GetMigrationStatusAsync() {
        return await _migrationRepository.GetAllAsync();
    }

    public async Task<int> RetryFailedMigrationsAsync() {
        try {
            _logger.LogInformation("Starting retry process for failed migrations");

            var failedRecords = await _migrationRepository.GetFailedMigrationsAsync(_configuration.MaxRetries);

            _logger.LogInformation($"Found {failedRecords.Count} failed migrations to retry");

            var retriedCount = 0;
            foreach (var record in failedRecords) {
                try {
                    var user = await _graphService.GetUserByIdAsync(record.MicrosoftUserId);
                    if (user != null) {
                        var result = await MigrateUserAsync(user);
                        if (result?.Status == MigrationStatus.Completed) {
                            retriedCount++;
                        }
                    }

                    // Small delay between retries
                    await Task.Delay(2000);
                } catch (Exception ex) {
                    _logger.LogError($"Error retrying migration for user {record.MicrosoftUserPrincipalName}: {ex.Message}");
                }
            }

            _logger.LogInformation($"Retry process completed. Successfully retried {retriedCount} out of {failedRecords.Count} failed migrations");

            return retriedCount;
        } catch (Exception ex) {
            _logger.LogError($"Error during retry process: {ex.Message}");
            throw;
        }
    }

    private bool ValidateRequiredAttributes(MicrosoftIdentityUser user) {
        var requiredAttribs = _configuration.RequiredAttributes;

        foreach (var attribute in requiredAttribs) {
            var value = attribute.ToLower() switch {
                "firstname" => user.GivenName,
                "lastname" => user.Surname,
                "email" => user.Mail,
                "contactid" => ExtractContactId(user),
                _ => null
            };

            if (string.IsNullOrWhiteSpace(value)) {
                _logger.LogWarning($"User {user.UserPrincipalName} missing required attribute: {attribute}");
                return false;
            }
        }

        return true;
    }

    private string ExtractContactId(MicrosoftIdentityUser user) {
        // Extract contact ID from user properties
        // This is business logic - customize as needed
        return user.Id; // For now, use the user ID as contact ID
    }

    private string ExtractUserIdFromLocation(string? location) {
        if (string.IsNullOrEmpty(location))
            return string.Empty;

        // Extract user ID from location header (e.g., "/Users/12345")
        var parts = location.Split('/');
        return parts.Length > 0 ? parts[^1] : string.Empty;
    }

    private Dictionary<string, object> BuildAdditionalAttributes(MicrosoftIdentityUser user) {
        var attributes = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(user.Department))
            attributes["department"] = user.Department;

        if (!string.IsNullOrEmpty(user.JobTitle))
            attributes["jobTitle"] = user.JobTitle;

        if (!string.IsNullOrEmpty(user.MobilePhone))
            attributes["phone"] = user.MobilePhone;

        if (!string.IsNullOrEmpty(user.CompanyName))
            attributes["company"] = user.CompanyName;

        return attributes;
    }
}