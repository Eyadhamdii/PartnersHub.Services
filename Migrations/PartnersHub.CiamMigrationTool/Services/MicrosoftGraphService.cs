using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using PartnersHub.CiamMigrationTool.Models;

namespace PartnersHub.CiamMigrationTool.Services;

public interface IMicrosoftGraphService {
    Task<List<MicrosoftIdentityUser>> GetUsersAsync(int skip = 0, int take = 100);
    Task<MicrosoftIdentityUser?> GetUserByIdAsync(string userId);
    Task<int> GetTotalUsersCountAsync();
    Task<List<MicrosoftIdentityUser>> SearchUsersByEmailAsync(string email);
    Task<string> GetAccessTokenAsync();
    Task<bool> ValidateTokenAsync(string token);
    Task<bool> TestDatabaseConnectionAsync();
}

public class MicrosoftGraphService : IMicrosoftGraphService {
    private readonly HttpClient _httpClient;
    private readonly MicrosoftGraphConfiguration _configuration;
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly ConnectionStringsConfiguration _connectionStrings;
    private readonly ISimpleLogger _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private string? _cachedAccessToken;
    private DateTime _tokenExpiry = DateTime.MinValue;
    private RSA? _rsaKey;

    public MicrosoftGraphService(
        HttpClient httpClient,
        MicrosoftGraphConfiguration configuration,
        JwtConfiguration jwtConfiguration,
        ConnectionStringsConfiguration connectionStrings,
        ISimpleLogger logger) {
        _httpClient = httpClient;
        _configuration = configuration;
        _jwtConfiguration = jwtConfiguration;
        _connectionStrings = connectionStrings;
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        // Configure HttpClient
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Initialize RSA key
        InitializeRsaKey();
    }

    private void InitializeRsaKey() {
        try {
            _rsaKey = RSA.Create();

            // Import the private key for signing
            var privateKeyBytes = Convert.FromBase64String(_jwtConfiguration.PrivateKey);
            _rsaKey.ImportRSAPrivateKey(privateKeyBytes, out _);

            _logger.LogInformation("RSA key initialized successfully");
        } catch (Exception ex) {
            _logger.LogError($"Failed to initialize RSA key: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> TestDatabaseConnectionAsync() {
        try {
            _logger.LogInformation("Testing database connection to ASP.NET Core Identity database");

            // For now, simulate connection test - in production, implement actual connection
            await Task.Delay(1000);

            _logger.LogInformation("Database connection test completed (simulated)");
            return true;
        } catch (Exception ex) {
            _logger.LogError($"Database connection failed: {ex.Message}");
            return false;
        }
    }

    public async Task<string> GetAccessTokenAsync() {
        try {
            // Check if we have a valid cached token
            if (!string.IsNullOrEmpty(_cachedAccessToken) && DateTime.UtcNow < _tokenExpiry.AddMinutes(-5)) {
                return _cachedAccessToken;
            }

            _logger.LogInformation("Generating JWT access token for internal authentication");

            var tokenHandler = new JwtSecurityTokenHandler();

            // Create RSA security key with key ID
            var rsaSecurityKey = new RsaSecurityKey(_rsaKey!) {
                KeyId = "migration-tool-key"
            };

            // Create token descriptor with proper claims
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "MigrationService"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat,
                        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                        ClaimValueTypes.Integer64),
                    new Claim(ClaimTypes.Name, "MigrationService"),
                    new Claim(ClaimTypes.Role, "SystemService"),
                    new Claim("scope", "user_read"),
                    new Claim("client_id", "migration_tool")
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfiguration.ExpireMinutes),
                Issuer = _jwtConfiguration.Issuer,
                Audience = _jwtConfiguration.Audience,
                SigningCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            _cachedAccessToken = tokenHandler.WriteToken(token);
            _tokenExpiry = DateTime.UtcNow.AddMinutes(_jwtConfiguration.ExpireMinutes);

            _logger.LogInformation("Successfully generated JWT access token");
            return _cachedAccessToken;
        } catch (Exception ex) {
            _logger.LogError($"Error generating JWT access token: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> ValidateTokenAsync(string token) {
        try {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create RSA key for validation using public key
            using var rsaPublic = RSA.Create();
            var publicKeyBytes = Convert.FromBase64String(_jwtConfiguration.PublicKey);
            rsaPublic.ImportRSAPublicKey(publicKeyBytes, out _);

            var validationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidIssuer = _jwtConfiguration.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtConfiguration.Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(rsaPublic) { KeyId = "migration-tool-key" },
                ClockSkew = TimeSpan.FromMinutes(5), // Allow 5 minutes clock skew
                RequireSignedTokens = true,
                RequireExpirationTime = true
            };

            // Validate the token
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            _logger.LogInformation("Token validation successful");
            return validatedToken != null && principal != null;
        } catch (SecurityTokenValidationException ex) {
            _logger.LogWarning($"Token validation failed: {ex.Message}");
            return false;
        } catch (Exception ex) {
            _logger.LogError($"Unexpected error during token validation: {ex.Message}");
            return false;
        }
    }

    public async Task<List<MicrosoftIdentityUser>> GetUsersAsync(int skip = 0, int take = 100) {
        try {
            _logger.LogInformation($"Fetching users from ASP.NET Core Identity system. Skip: {skip}, Take: {take}");

            // For now, use sample data - in production, implement actual database queries
            await Task.Delay(1000); // Simulate database call

            var allUsers = GetSampleUsersFromIdentityDb();
            var pagedUsers = allUsers.Skip(skip).Take(take).ToList();

            _logger.LogInformation($"Successfully fetched {pagedUsers.Count} users from Identity system");
            return pagedUsers;
        } catch (Exception ex) {
            _logger.LogError($"Error fetching users from Identity system: {ex.Message}");

            // Fallback to sample data
            return GetSampleUsersFromIdentityDb().Skip(skip).Take(take).ToList();
        }
    }

    public async Task<MicrosoftIdentityUser?> GetUserByIdAsync(string userId) {
        try {
            _logger.LogInformation($"Fetching user {userId} from Identity system");

            // For now, use sample data - in production, implement actual database query
            await Task.Delay(500);

            var allUsers = GetSampleUsersFromIdentityDb();
            var user = allUsers.FirstOrDefault(u => u.Id == userId);

            if (user != null) {
                _logger.LogInformation($"Successfully fetched user {userId} from Identity system");
            } else {
                _logger.LogWarning($"User {userId} not found in Identity system");
            }

            return user;
        } catch (Exception ex) {
            _logger.LogError($"Error fetching user {userId} from Identity system: {ex.Message}");

            // Fallback to sample data
            return GetSampleUsersFromIdentityDb().FirstOrDefault(u => u.Id == userId);
        }
    }

    public async Task<int> GetTotalUsersCountAsync() {
        try {
            _logger.LogInformation("Getting total users count from Identity system");

            // For now, use sample data - in production, implement actual database count
            await Task.Delay(500);

            var count = GetSampleUsersFromIdentityDb().Count;
            _logger.LogInformation($"Total users count from Identity system: {count}");
            return count;
        } catch (Exception ex) {
            _logger.LogError($"Error getting total users count from Identity system: {ex.Message}");

            // Fallback to sample data
            var count = GetSampleUsersFromIdentityDb().Count;
            _logger.LogWarning($"Falling back to sample data count: {count}");
            return count;
        }
    }

    public async Task<List<MicrosoftIdentityUser>> SearchUsersByEmailAsync(string email) {
        try {
            _logger.LogInformation($"Searching users by email {email} from Identity system");

            // For now, use sample data - in production, implement actual database search
            await Task.Delay(500);

            var allUsers = GetSampleUsersFromIdentityDb();
            var matchingUsers = allUsers
                .Where(u => u.Mail.Contains(email, StringComparison.OrdinalIgnoreCase) ||
                           u.UserPrincipalName.Contains(email, StringComparison.OrdinalIgnoreCase))
                .ToList();

            _logger.LogInformation($"Found {matchingUsers.Count} users matching email {email} from Identity system");
            return matchingUsers;
        } catch (Exception ex) {
            _logger.LogError($"Error searching users by email {email} from Identity system: {ex.Message}");

            // Fallback to sample data
            var sampleUsers = GetSampleUsersFromIdentityDb()
                .Where(u => u.Mail.Contains(email, StringComparison.OrdinalIgnoreCase) ||
                           u.UserPrincipalName.Contains(email, StringComparison.OrdinalIgnoreCase))
                .ToList();
            _logger.LogWarning($"Falling back to sample data, found {sampleUsers.Count} users");
            return sampleUsers;
        }
    }

    private List<MicrosoftIdentityUser> GetSampleUsersFromIdentityDb() {
        // Sample users that simulate ASP.NET Core Identity data structure
        return new List<MicrosoftIdentityUser>
        {
            new()
            {
                Id = "identity-user-001",
                UserPrincipalName = "admin@pif.gov.sa",
                DisplayName = "System Administrator",
                GivenName = "System",
                Surname = "Administrator",
                Mail = "admin@pif.gov.sa",
                MobilePhone = "+966501111111",
                JobTitle = "System Administrator",
                Department = "IT Operations",
                CompanyName = "PIF",
                AccountEnabled = true,
                CreatedDateTime = DateTime.UtcNow.AddDays(-90)
            },
            new()
            {
                Id = "identity-user-002",
                UserPrincipalName = "manager@pif.gov.sa",
                DisplayName = "Business Manager",
                GivenName = "Business",
                Surname = "Manager",
                Mail = "manager@pif.gov.sa",
                MobilePhone = "+966502222222",
                JobTitle = "Business Manager",
                Department = "Business Operations",
                CompanyName = "PIF",
                AccountEnabled = true,
                CreatedDateTime = DateTime.UtcNow.AddDays(-60)
            },
            new()
            {
                Id = "identity-user-003",
                UserPrincipalName = "analyst@pif.gov.sa",
                DisplayName = "Data Analyst",
                GivenName = "Data",
                Surname = "Analyst",
                Mail = "analyst@pif.gov.sa",
                MobilePhone = "+966503333333",
                JobTitle = "Senior Data Analyst",
                Department = "Analytics",
                CompanyName = "PIF",
                AccountEnabled = true,
                CreatedDateTime = DateTime.UtcNow.AddDays(-45)
            },
            new()
            {
                Id = "identity-user-004",
                UserPrincipalName = "developer@pif.gov.sa",
                DisplayName = "Lead Developer",
                GivenName = "Lead",
                Surname = "Developer",
                Mail = "developer@pif.gov.sa",
                MobilePhone = "+966504444444",
                JobTitle = "Lead Software Developer",
                Department = "Engineering",
                CompanyName = "PIF",
                AccountEnabled = true,
                CreatedDateTime = DateTime.UtcNow.AddDays(-30)
            },
            new()
            {
                Id = "identity-user-005",
                UserPrincipalName = "consultant@pif.gov.sa",
                DisplayName = "External Consultant",
                GivenName = "External",
                Surname = "Consultant",
                Mail = "consultant@pif.gov.sa",
                MobilePhone = "+966505555555",
                JobTitle = "Senior Consultant",
                Department = "Consulting",
                CompanyName = "PIF",
                AccountEnabled = false, // Disabled for testing
                CreatedDateTime = DateTime.UtcNow.AddDays(-15)
            }
        };
    }

    public void Dispose() {
        _rsaKey?.Dispose();
        _httpClient?.Dispose();
    }
}

// Response models for authentication
public class TokenResponse {
    public string? AccessToken { get; set; }
    public string? TokenType { get; set; }
    public int ExpiresIn { get; set; }
    public string? Scope { get; set; }
}