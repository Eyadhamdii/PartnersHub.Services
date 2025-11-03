using System.Text;
using System.Text.Json;
using PartnersHub.CiamMigrationTool.Models;

namespace PartnersHub.CiamMigrationTool.Services;

public interface ICiamService {
    Task<string> GetAccessTokenAsync();
    Task<CiamUserResponse?> CreateUserAsync(CiamUser user, string accessToken);
    Task<CiamUserResponse?> GetUserByUsernameAsync(string username, string accessToken);
    Task<CiamUserResponse?> GetUserByEmailAsync(string email, string accessToken);
    Task<CiamBulkResponse?> CreateUsersBulkAsync(List<CiamUser> users, string accessToken);
}

public class CiamService : ICiamService {
    private readonly HttpClient _httpClient;
    private readonly CiamConfiguration _configuration;
    private readonly ISimpleLogger _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public CiamService(HttpClient httpClient, CiamConfiguration configuration, ISimpleLogger logger) {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        // Configure HttpClient for CIAM-specific requirements
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "PartnersHub-CIAM-MigrationTool/1.0");

        _jsonOptions = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<string> GetAccessTokenAsync() {
        try {
            _logger.LogInformation("[TOKEN] Getting access token from CIAM");

            // Prepare the request with proper CIAM authentication
            var request = new HttpRequestMessage(HttpMethod.Post, _configuration.TokenUrl);

            // Use client credentials in the request body (preferred method)
            var formParams = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "client_credentials"),
                new("client_id", _configuration.ClientId),
                new("client_secret", _configuration.ClientSecret),
                new("scope", _configuration.Scopes)
            };

            request.Content = new FormUrlEncodedContent(formParams);
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                _logger.LogError($"[FAIL] Failed to get access token. Status: {response.StatusCode}");
                _logger.LogError($"[FAIL] Response: {responseContent}");
                throw new HttpRequestException($"Failed to get access token: {response.StatusCode} - {responseContent}");
            }

            var tokenResponse = JsonSerializer.Deserialize<CiamTokenResponse>(responseContent, _jsonOptions);

            if (tokenResponse?.AccessToken == null) {
                _logger.LogError("[FAIL] Token response is null or missing access token");
                throw new InvalidOperationException("Access token is null in response");
            }

            _logger.LogInformation("[SUCCESS] Successfully obtained access token");
            return tokenResponse.AccessToken;
        } catch (Exception ex) {
            _logger.LogError($"[ERROR] Error getting access token from CIAM: {ex.Message}");
            throw;
        }
    }

    public async Task<CiamBulkResponse?> CreateUsersBulkAsync(List<CiamUser> users, string accessToken) {
        try {
            _logger.LogInformation($"[BULK] Creating {users.Count} users in bulk via CIAM SCIM API");

            var bulkRequest = new CiamBulkRequest {
                Schemas = new List<string> { "urn:ietf:params:scim:api:messages:2.0:BulkRequest" },
                Operations = users.Select(user => new CiamBulkOperation {
                    Method = "POST",
                    Path = "/Users",
                    Data = user,
                    BulkId = Guid.NewGuid().ToString()
                }).ToList()
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration.BaseUrl}{_configuration.BulkApiEndpoint}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var json = JsonSerializer.Serialize(bulkRequest, _jsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                _logger.LogError($"[FAIL] Failed to create users in bulk. Status: {response.StatusCode}, Error: {content}");
                throw new HttpRequestException($"Failed to create users in bulk: {response.StatusCode} - {content}");
            }

            var bulkResponse = JsonSerializer.Deserialize<CiamBulkResponse>(content, _jsonOptions);
            _logger.LogInformation($"[SUCCESS] Successfully created {bulkResponse?.Operations?.Count ?? 0} users in bulk");

            return bulkResponse;
        } catch (Exception ex) {
            _logger.LogError($"[ERROR] Error creating users in bulk via CIAM: {ex.Message}");
            throw;
        }
    }

    public async Task<CiamUserResponse?> CreateUserAsync(CiamUser user, string accessToken) {
        try {
            _logger.LogInformation($"[USER] Creating user {user.UserName} in CIAM");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration.BaseUrl}/Users");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var json = SerializeCiamUser(user);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                _logger.LogError($"[FAIL] Failed to create user {user.UserName}. Status: {response.StatusCode}, Error: {content}");
                throw new HttpRequestException($"Failed to create user: {response.StatusCode} - {content}");
            }

            var userResponse = JsonSerializer.Deserialize<CiamUserResponse>(content, _jsonOptions);
            _logger.LogInformation($"[SUCCESS] Successfully created user {user.UserName} with ID {userResponse?.Id}");

            return userResponse;
        } catch (Exception ex) {
            _logger.LogError($"[ERROR] Error creating user {user.UserName} in CIAM: {ex.Message}");
            throw;
        }
    }

    public async Task<CiamUserResponse?> GetUserByUsernameAsync(string username, string accessToken) {
        try {
            _logger.LogInformation($"[USER] Getting user by username {username} from CIAM");

            var encodedFilter = Uri.EscapeDataString($"username eq '{username}'");
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration.BaseUrl}/Users?filter={encodedFilter}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                _logger.LogError($"[FAIL] Failed to get user by username {username}. Status: {response.StatusCode}, Error: {content}");
                return null;
            }

            var searchResponse = JsonSerializer.Deserialize<CiamSearchResponse>(content, _jsonOptions);
            var user = searchResponse?.Resources?.FirstOrDefault();

            _logger.LogInformation($"[SUCCESS] User search by username {username} returned {searchResponse?.Resources?.Count ?? 0} results");

            return user;
        } catch (Exception ex) {
            _logger.LogError($"[ERROR] Error getting user by username {username} from CIAM: {ex.Message}");
            return null;
        }
    }

    public async Task<CiamUserResponse?> GetUserByEmailAsync(string email, string accessToken) {
        try {
            _logger.LogInformation($"[USER] Getting user by email {email} from CIAM");

            var encodedFilter = Uri.EscapeDataString($"emails eq '{email}'");
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration.BaseUrl}/Users?filter={encodedFilter}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) {
                _logger.LogError($"[FAIL] Failed to get user by email {email}. Status: {response.StatusCode}, Error: {content}");
                return null;
            }

            var searchResponse = JsonSerializer.Deserialize<CiamSearchResponse>(content, _jsonOptions);
            var user = searchResponse?.Resources?.FirstOrDefault();

            _logger.LogInformation($"[SUCCESS] User search by email {email} returned {searchResponse?.Resources?.Count ?? 0} results");

            return user;
        } catch (Exception ex) {
            _logger.LogError($"[ERROR] Error getting user by email {email} from CIAM: {ex.Message}");
            return null;
        }
    }

    private string SerializeCiamUser(CiamUser user) {
        // Custom serialization to handle CIAM-specific property names
        var ciamObject = new Dictionary<string, object> {
            ["schemas"] = user.Schemas,
            ["name"] = user.Name,
            ["userName"] = user.UserName,
            ["displayName"] = user.DisplayName,
            ["emails"] = user.Emails,
            ["phoneNumbers"] = user.PhoneNumbers,
            ["urn:ietf:params:scim:schemas:extension:enterprise:2.0:User"] = user.EnterpriseExtension,
            ["urn:scim:wso2:schema"] = user.Wso2Extension,
            ["urn:scim:schemas:extension:custom:User"] = user.CustomExtension
        };

        return JsonSerializer.Serialize(ciamObject, _jsonOptions);
    }
}

// Models for CIAM operations
public class CiamBulkRequest {
    public List<string> Schemas { get; set; } = new();
    public List<CiamBulkOperation> Operations { get; set; } = new();
}

public class CiamBulkOperation {
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string BulkId { get; set; } = string.Empty;
    public object? Data { get; set; }
}

public class CiamBulkResponse {
    public List<string> Schemas { get; set; } = new();
    public List<CiamBulkOperationResult> Operations { get; set; } = new();
}

public class CiamBulkOperationResult {
    public string BulkId { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Location { get; set; }
    public object? Response { get; set; }
}

public class CiamInvitation {
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ContactId { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();
}

public class CiamInvitationResponse {
    public string InvitationId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string InvitationUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}