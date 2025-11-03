using System.Text.Json;
using PartnersHub.CiamMigrationTool.Models;
using System.Text;

namespace PartnersHub.CiamMigrationTool.Services;

public interface ICiamService
{
    Task<string> GetAccessTokenAsync();
    Task<CiamUserResponse?> CreateUserAsync(CiamUser user, string accessToken);
    Task<CiamUserResponse?> GetUserByUsernameAsync(string username, string accessToken);
    Task<CiamUserResponse?> GetUserByEmailAsync(string email, string accessToken);
    Task<bool> ValidateUserAsync(string userId, string accessToken);
    Task<CiamBulkResponse?> CreateUsersBulkAsync(List<CiamUser> users, string accessToken);
    Task<CiamInvitationResponse?> SendInvitationAsync(CiamInvitation invitation, string accessToken);
    Task<CiamUserInfoResponse?> GetUserInfoAsync(string accessToken);
    Task<CiamIntrospectResponse?> IntrospectTokenAsync(string token, string accessToken);
    Task<bool> TestCiamFeaturesAsync(string accessToken);
    Task<bool> TestCiamConnectivityAsync();
    Task<CiamDiscoveryResponse?> GetDiscoveryDocumentAsync();
}

public class CiamService : ICiamService
{
    private readonly HttpClient _httpClient;
    private readonly CiamConfiguration _configuration;
    private readonly ISimpleLogger _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public CiamService(HttpClient httpClient, CiamConfiguration configuration, ISimpleLogger logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        
        // Configure HttpClient for CIAM-specific requirements
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "PartnersHub-CIAM-MigrationTool/1.0");
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<bool> TestCiamConnectivityAsync()
    {
        try
        {
            _logger.LogInformation("[CONNECTIVITY] Testing basic CIAM connectivity...");
            _logger.LogInformation($"[CONNECTIVITY] Base URL: {_configuration.BaseUrl}");
            _logger.LogInformation($"[CONNECTIVITY] Token URL: {_configuration.TokenUrl}");

            // Use DiscoveryUrl if configured, otherwise construct it
            var discoveryUrl = !string.IsNullOrEmpty(_configuration.DiscoveryUrl) 
                ? _configuration.DiscoveryUrl 
                : $"{_configuration.TokenUrl}/.well-known/openid-configuration";
                
            _logger.LogInformation($"[CONNECTIVITY] Testing discovery endpoint: {discoveryUrl}");

            var response = await _httpClient.GetAsync(discoveryUrl);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"[SUCCESS] Discovery endpoint is accessible");
                _logger.LogInformation($"[SUCCESS] Response length: {content.Length} characters");
                return true;
            }
            else
            {
                _logger.LogError($"[FAIL] Discovery endpoint failed. Status: {response.StatusCode}");
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"[FAIL] Error response: {errorContent}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"[ERROR] Connectivity test failed: {ex.Message}");
            return false;
        }
    }

    public async Task<CiamDiscoveryResponse?> GetDiscoveryDocumentAsync()
    {
        try
        {
            _logger.LogInformation("[DISCOVERY] Getting CIAM discovery document...");
            
            // Use DiscoveryUrl if configured, otherwise construct it
            var discoveryUrl = !string.IsNullOrEmpty(_configuration.DiscoveryUrl) 
                ? _configuration.DiscoveryUrl 
                : $"{_configuration.TokenUrl}/.well-known/openid-configuration";
                
            var response = await _httpClient.GetAsync(discoveryUrl);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"[FAIL] Failed to get discovery document. Status: {response.StatusCode}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var discovery = JsonSerializer.Deserialize<CiamDiscoveryResponse>(content, _jsonOptions);
            
            if (discovery != null)
            {
                _logger.LogInformation($"[SUCCESS] Discovery document retrieved");
                _logger.LogInformation($"[DISCOVERY] Issuer: {discovery.Issuer}");
                _logger.LogInformation($"[DISCOVERY] Token Endpoint: {discovery.TokenEndpoint}");
                _logger.LogInformation($"[DISCOVERY] UserInfo Endpoint: {discovery.UserinfoEndpoint}");
                _logger.LogInformation($"[DISCOVERY] Grant Types: {string.Join(", ", discovery.GrantTypesSupported ?? new List<string>())}");
            }
            
            return discovery;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[ERROR] Failed to get discovery document: {ex.Message}");
            return null;
        }
    }

    public async Task<string> GetAccessTokenAsync()
    {
        try
        {
            _logger.LogInformation("[TOKEN] Getting access token from CIAM");
            _logger.LogInformation($"[TOKEN] Client ID: {_configuration.ClientId}");
            _logger.LogInformation($"[TOKEN] Token URL: {_configuration.TokenUrl}");
            _logger.LogInformation($"[TOKEN] Requested Scopes: {_configuration.Scopes}");

            // First test connectivity
            var connectivityOk = await TestCiamConnectivityAsync();
            if (!connectivityOk)
            {
                throw new HttpRequestException("CIAM connectivity test failed");
            }

            // Get discovery document to verify the correct token endpoint
            var discovery = await GetDiscoveryDocumentAsync();
            
            // Use discovery token endpoint if available and not empty, otherwise fall back to configured URL
            var tokenEndpoint = _configuration.TokenUrl;
            if (discovery?.TokenEndpoint != null && !string.IsNullOrEmpty(discovery.TokenEndpoint))
            {
                tokenEndpoint = discovery.TokenEndpoint;
                _logger.LogInformation($"[TOKEN] Using discovery token endpoint: {tokenEndpoint}");
            }
            else
            {
                _logger.LogInformation($"[TOKEN] Discovery endpoint empty, using configured: {tokenEndpoint}");
            }

            // Prepare the request with proper CIAM authentication
            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
            
            // CIAM typically expects client credentials in the request body, not Basic auth
            var formParams = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "client_credentials"),
                new("client_id", _configuration.ClientId),
                new("client_secret", _configuration.ClientSecret),
                new("scope", _configuration.Scopes)
            };

            request.Content = new FormUrlEncodedContent(formParams);
            request.Headers.Add("Accept", "application/json");

            _logger.LogInformation("[TOKEN] Sending token request...");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation($"[TOKEN] Response Status: {response.StatusCode}");
            _logger.LogInformation($"[TOKEN] Response Length: {responseContent.Length}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"[FAIL] Failed to get access token. Status: {response.StatusCode}");
                _logger.LogError($"[FAIL] Response: {responseContent}");
                
                // Try alternative authentication method (Basic Auth)
                return await TryBasicAuthTokenRequestAsync(tokenEndpoint);
            }

            var tokenResponse = JsonSerializer.Deserialize<CiamTokenResponse>(responseContent, _jsonOptions);
            
            if (tokenResponse?.AccessToken == null)
            {
                _logger.LogError("[FAIL] Token response is null or missing access token");
                _logger.LogError($"[FAIL] Raw response: {responseContent}");
                throw new InvalidOperationException("Access token is null in response");
            }

            _logger.LogInformation("[SUCCESS] Successfully obtained access token");
            _logger.LogInformation($"[SUCCESS] Token type: {tokenResponse.TokenType}");
            _logger.LogInformation($"[SUCCESS] Expires in: {tokenResponse.ExpiresIn} seconds");
            _logger.LogInformation($"[SUCCESS] Token length: {tokenResponse.AccessToken.Length}");
            
            return tokenResponse.AccessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[ERROR] Error getting access token from CIAM: {ex.Message}");
            throw;
        }
    }

    private async Task<string> TryBasicAuthTokenRequestAsync(string tokenEndpoint)
    {
        try
        {
            _logger.LogInformation("[TOKEN] Trying alternative Basic Auth method...");

            var authValue = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_configuration.ClientId}:{_configuration.ClientSecret}"));
            
            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authValue);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", _configuration.Scopes)
            });

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"[FAIL] Basic auth also failed. Status: {response.StatusCode}, Error: {content}");
                throw new HttpRequestException($"Failed to get access token with both methods: {response.StatusCode} - {content}");
            }

            var tokenResponse = JsonSerializer.Deserialize<CiamTokenResponse>(content, _jsonOptions);
            
            _logger.LogInformation("[SUCCESS] Successfully obtained access token using Basic Auth");
            return tokenResponse?.AccessToken ?? throw new InvalidOperationException("Access token is null");
        }
        catch (Exception ex)
        {
            _logger.LogError($"[ERROR] Basic auth token request failed: {ex.Message}");
            throw;
        }
    }

    public async Task<CiamBulkResponse?> CreateUsersBulkAsync(List<CiamUser> users, string accessToken)
    {
        try
        {
            _logger.LogInformation($"[BULK] Creating {users.Count} users in bulk via CIAM SCIM API");

            var bulkRequest = new CiamBulkRequest
            {
                Schemas = new List<string> { "urn:ietf:params:scim:api:messages:2.0:BulkRequest" },
                Operations = users.Select(user => new CiamBulkOperation
                {
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

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"[FAIL] Failed to create users in bulk. Status: {response.StatusCode}, Error: {content}");
                throw new HttpRequestException($"Failed to create users in bulk: {response.StatusCode} - {content}");
            }

            var bulkResponse = JsonSerializer.Deserialize<CiamBulkResponse>(content, _jsonOptions);
            _logger.LogInformation($"[SUCCESS] Successfully created {bulkResponse?.Operations?.Count ?? 0} users in bulk");
            
            return bulkResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[ERROR] Error creating users in bulk via CIAM: {ex.Message}");
            throw;
        }
    }

    public async Task<CiamInvitationResponse?> SendInvitationAsync(CiamInvitation invitation, string accessToken)
    {
        try
        {
            _logger.LogInformation($"[INVITATION] Sending invitation to {invitation.Email} via CIAM");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration.BaseUrl}{_configuration.InvitationEndpoint}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            
            var json = JsonSerializer.Serialize(invitation, _jsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"[FAIL] Failed to send invitation to {invitation.Email}. Status: {response.StatusCode}, Error: {content}");
                throw new HttpRequestException($"Failed to send invitation: {response.StatusCode} - {content}");
            }

            var invitationResponse = JsonSerializer.Deserialize<CiamInvitationResponse>(content, _jsonOptions);
            _logger.LogInformation($"[SUCCESS] Successfully sent invitation to {invitation.Email} with ID {invitationResponse?.InvitationId}");
            
            return invitationResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[ERROR] Error sending invitation to {invitation.Email} via CIAM: {ex.Message}");
            throw;
        }
    }

    public async Task<CiamUserInfoResponse?> GetUserInfoAsync(string accessToken)
    {
        try
        {
            _logger.LogInformation("[USERINFO] Getting user info from CIAM");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration.BaseUrl}{_configuration.UserInfoEndpoint}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"[FAIL] Failed to get user info. Status: {response.StatusCode}, Error: {content}");
                return null;
            }

            var userInfo = JsonSerializer.Deserialize<CiamUserInfoResponse>(content, _jsonOptions);
            _logger.LogInformation($"[SUCCESS] Successfully retrieved user info for subject: {userInfo?.Subject}");
            
            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[ERROR] Error getting user info from CIAM: {ex.Message}");
            return null;
        }
    }

    public async Task<CiamIntrospectResponse?> IntrospectTokenAsync(string token, string accessToken)
    {
        try
        {
            _logger.LogInformation("[INTROSPECT] Introspecting token via CIAM");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration.BaseUrl}{_configuration.IntrospectEndpoint}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("token", token)
            });

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"[FAIL] Failed to introspect token. Status: {response.StatusCode}, Error: {content}");
                return null;
            }

            var introspectResponse = JsonSerializer.Deserialize<CiamIntrospectResponse>(content, _jsonOptions);
            _logger.LogInformation($"[SUCCESS] Token introspection result: Active={introspectResponse?.Active}");
            
            return introspectResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[ERROR] Error introspecting token via CIAM: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> TestCiamFeaturesAsync(string accessToken)
    {
        try
        {
            _logger.LogInformation("[FEATURES] Testing CIAM features and capabilities");

            var features = _configuration.Features;
            var supportedClaims = _configuration.SupportedClaims;
            var version = _configuration.Version;

            _logger.LogInformation($"[FEATURES] CIAM Version: {version}");
            _logger.LogInformation($"[FEATURES] Login Support: {features.Login}");
            _logger.LogInformation($"[FEATURES] Forgot/Reset Password: {features.ForgotResetPassword}");
            _logger.LogInformation($"[FEATURES] Email Verification: {features.EmailVerification}");
            _logger.LogInformation($"[FEATURES] Account Activation: {features.AccountActivation}");
            _logger.LogInformation($"[FEATURES] OTP/2FA: {features.OtpTwoFactorAuth}");
            _logger.LogInformation($"[FEATURES] Session Timeout: {features.SessionTimeout}");
            _logger.LogInformation($"[FEATURES] Configurable Redirects: {features.ConfigurableRedirects}");
            _logger.LogInformation($"[FEATURES] Custom Branding: {features.CustomBranding}");
            _logger.LogInformation($"[FEATURES] Arabic/English Support: {features.ArabicEnglishSupport}");
            _logger.LogInformation($"[FEATURES] RTL Support: {features.RtlSupport}");
            _logger.LogInformation($"[FEATURES] Supported Claims: {string.Join(", ", supportedClaims)}");

            // Test UserInfo endpoint if we have a valid token
            try
            {
                var userInfo = await GetUserInfoAsync(accessToken);
                var userInfoWorking = userInfo != null;
                _logger.LogInformation($"[FEATURES] UserInfo Endpoint: {(userInfoWorking ? "[OK] Working" : "[FAIL] Failed")}");
                return userInfoWorking;
            }
            catch
            {
                _logger.LogInformation("[FEATURES] UserInfo endpoint test skipped (client credentials token)");
                return true; // Client credentials tokens typically can't access userinfo
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"[ERROR] Error testing CIAM features: {ex.Message}");
            return false;
        }
    }

    public async Task<CiamUserResponse?> CreateUserAsync(CiamUser user, string accessToken)
    {
        try
        {
            _logger.LogInformation($"[USER] Creating user {user.UserName} in CIAM");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration.BaseUrl}/Users");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            
            var json = SerializeCiamUser(user);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"[FAIL] Failed to create user {user.UserName}. Status: {response.StatusCode}, Error: {content}");
                throw new HttpRequestException($"Failed to create user: {response.StatusCode} - {content}");
            }

            var userResponse = JsonSerializer.Deserialize<CiamUserResponse>(content, _jsonOptions);
            _logger.LogInformation($"[SUCCESS] Successfully created user {user.UserName} with ID {userResponse?.Id}");
            
            return userResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[ERROR] Error creating user {user.UserName} in CIAM: {ex.Message}");
            throw;
        }
    }

    public async Task<CiamUserResponse?> GetUserByUsernameAsync(string username, string accessToken)
    {
        try
        {
            _logger.LogInformation($"[USER] Getting user by username {username} from CIAM");

            var encodedFilter = Uri.EscapeDataString($"username eq '{username}'");
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration.BaseUrl}/Users?filter={encodedFilter}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"[FAIL] Failed to get user by username {username}. Status: {response.StatusCode}, Error: {content}");
                return null;
            }

            var searchResponse = JsonSerializer.Deserialize<CiamSearchResponse>(content, _jsonOptions);
            var user = searchResponse?.Resources?.FirstOrDefault();
            
            _logger.LogInformation($"[SUCCESS] User search by username {username} returned {searchResponse?.Resources?.Count ?? 0} results");
            
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[ERROR] Error getting user by username {username} from CIAM: {ex.Message}");
            return null;
        }
    }

    public async Task<CiamUserResponse?> GetUserByEmailAsync(string email, string accessToken)
    {
        try
        {
            _logger.LogInformation($"[USER] Getting user by email {email} from CIAM");

            var encodedFilter = Uri.EscapeDataString($"emails eq '{email}'");
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration.BaseUrl}/Users?filter={encodedFilter}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"[FAIL] Failed to get user by email {email}. Status: {response.StatusCode}, Error: {content}");
                return null;
            }

            var searchResponse = JsonSerializer.Deserialize<CiamSearchResponse>(content, _jsonOptions);
            var user = searchResponse?.Resources?.FirstOrDefault();
            
            _logger.LogInformation($"[SUCCESS] User search by email {email} returned {searchResponse?.Resources?.Count ?? 0} results");
            
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[ERROR] Error getting user by email {email} from CIAM: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> ValidateUserAsync(string userId, string accessToken)
    {
        try
        {
            _logger.LogInformation($"[USER] Validating user {userId} in CIAM");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration.BaseUrl}/Users/{userId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"[SUCCESS] User {userId} validation successful");
                return true;
            }
            else
            {
                _logger.LogWarning($"[FAIL] User {userId} validation failed. Status: {response.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"[ERROR] Error validating user {userId} in CIAM: {ex.Message}");
            return false;
        }
    }

    private string SerializeCiamUser(CiamUser user)
    {
        // Custom serialization to handle CIAM-specific property names
        var ciamObject = new Dictionary<string, object>
        {
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

// New models for enhanced CIAM features
public class CiamDiscoveryResponse
{
    public string Issuer { get; set; } = string.Empty;
    public string AuthorizationEndpoint { get; set; } = string.Empty;
    public string TokenEndpoint { get; set; } = string.Empty;
    public string UserinfoEndpoint { get; set; } = string.Empty;
    public string JwksUri { get; set; } = string.Empty;
    public List<string>? ScopesSupported { get; set; }
    public List<string>? ResponseTypesSupported { get; set; }
    public List<string>? GrantTypesSupported { get; set; }
    public List<string>? SubjectTypesSupported { get; set; }
    public List<string>? IdTokenSigningAlgValuesSupported { get; set; }
    public List<string>? ClaimsSupported { get; set; }
}

public class CiamBulkRequest
{
    public List<string> Schemas { get; set; } = new();
    public List<CiamBulkOperation> Operations { get; set; } = new();
}

public class CiamBulkOperation
{
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string BulkId { get; set; } = string.Empty;
    public object? Data { get; set; }
}

public class CiamBulkResponse
{
    public List<string> Schemas { get; set; } = new();
    public List<CiamBulkOperationResult> Operations { get; set; } = new();
}

public class CiamBulkOperationResult
{
    public string BulkId { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Location { get; set; }
    public object? Response { get; set; }
}

public class CiamInvitation
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ContactId { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();
}

public class CiamInvitationResponse
{
    public string InvitationId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string InvitationUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class CiamUserInfoResponse
{
    public string Subject { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public List<string> Groups { get; set; } = new();
    public string CompanyId { get; set; } = string.Empty;
    public string ContactId { get; set; } = string.Empty;
}

public class CiamIntrospectResponse
{
    public bool Active { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public long Exp { get; set; }
    public long Iat { get; set; }
}