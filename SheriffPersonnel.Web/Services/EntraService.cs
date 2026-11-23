using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.Web.Services
{
    public class EntraService : IEntraService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EntraService> _logger;
        private readonly ITokenAcquisition? _tokenAcquisition;
        private IConfidentialClientApplication? _confidentialClientApp;
        private readonly SemaphoreSlim _appLock = new(1, 1);

        public EntraService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<EntraService> logger,
            IServiceProvider serviceProvider)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _tokenAcquisition = (ITokenAcquisition?)serviceProvider.GetService(typeof(ITokenAcquisition));
        }

        public async Task<IEnumerable<EntraUserDto>> SearchUsersAsync(string query, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Enumerable.Empty<EntraUserDto>();
            }

            var cleanQuery = Regex.Replace(query, @"[""\\]", " ").Trim();
            if (cleanQuery.Length < 2)
            {
                return Enumerable.Empty<EntraUserDto>();
            }

            try
            {
                var token = await GetAccessTokenAsync(cancellationToken);
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Unable to acquire access token for Microsoft Graph.");
                    return Enumerable.Empty<EntraUserDto>();
                }

                var words = cleanQuery.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                string searchClause;
                if (words.Length == 1)
                {
                    searchClause = $"\"displayName:{words[0]}\" OR \"userPrincipalName:{words[0]}\"";
                }
                else
                {
                    var displayNameClauses = string.Join(" AND ", words.Select(w => $"\"displayName:{w}\""));
                    searchClause = $"({displayNameClauses}) OR \"userPrincipalName:{cleanQuery}\"";
                }

                var encodedSearch = Uri.EscapeDataString(searchClause);
                var requestUrl = $"https://graph.microsoft.com/v1.0/users?$search={encodedSearch}&$count=true&$top=20&$select=id,displayName,userPrincipalName,mail,jobTitle,department";

                var httpClient = _httpClientFactory.CreateClient();
                using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.Add("ConsistencyLevel", "eventual");

                using var response = await httpClient.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogWarning("Microsoft Graph user search returned status code {StatusCode}. Error: {Error}", response.StatusCode, errorBody);
                    return Enumerable.Empty<EntraUserDto>();
                }

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var graphResponse = JsonSerializer.Deserialize<GraphResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (graphResponse?.Value == null || graphResponse.Value.Count == 0)
                {
                    return Enumerable.Empty<EntraUserDto>();
                }

                var results = graphResponse.Value
                    .Where(u => !string.IsNullOrWhiteSpace(u.DisplayName))
                    .Select(u => new EntraUserDto
                    {
                        Id = u.Id ?? string.Empty,
                        DisplayName = u.DisplayName ?? string.Empty,
                        UserPrincipalName = !string.IsNullOrWhiteSpace(u.UserPrincipalName) ? u.UserPrincipalName : (u.Mail ?? string.Empty),
                        Mail = u.Mail,
                        JobTitle = u.JobTitle,
                        Department = u.Department
                    })
                    .OrderBy(u => u.DisplayName)
                    .ToList();

                return results;
            }
            catch (OperationCanceledException)
            {
                return Enumerable.Empty<EntraUserDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while searching Entra users for query: {Query}", query);
                return Enumerable.Empty<EntraUserDto>();
            }
        }

        private async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            if (_tokenAcquisition != null)
            {
                try
                {
                    var token = await _tokenAcquisition.GetAccessTokenForAppAsync("https://graph.microsoft.com/.default");
                    if (!string.IsNullOrEmpty(token))
                    {
                        return token;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "ITokenAcquisition.GetAccessTokenForAppAsync failed; falling back to MSAL ConfidentialClientApplication.");
                }
            }

            return await GetDirectClientTokenAsync(cancellationToken);
        }

        private async Task<string?> GetDirectClientTokenAsync(CancellationToken cancellationToken)
        {
            if (_confidentialClientApp == null)
            {
                await _appLock.WaitAsync(cancellationToken);
                try
                {
                    if (_confidentialClientApp == null)
                    {
                        var clientId = _configuration["AzureAd:ClientId"];
                        var clientSecret = _configuration["AzureAd:ClientSecret"];
                        var tenantId = _configuration["AzureAd:TenantId"];
                        var instance = _configuration["AzureAd:Instance"] ?? "https://login.microsoftonline.com/";

                        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret) || string.IsNullOrWhiteSpace(tenantId))
                        {
                            _logger.LogError("AzureAd configuration is missing ClientId, ClientSecret, or TenantId.");
                            return null;
                        }

                        _confidentialClientApp = ConfidentialClientApplicationBuilder.Create(clientId)
                            .WithClientSecret(clientSecret)
                            .WithAuthority(new Uri(new Uri(instance), tenantId))
                            .Build();
                    }
                }
                finally
                {
                    _appLock.Release();
                }
            }

            var authResult = await _confidentialClientApp
                .AcquireTokenForClient(new[] { "https://graph.microsoft.com/.default" })
                .ExecuteAsync(cancellationToken);

            return authResult.AccessToken;
        }

        private class GraphResponse
        {
            [JsonPropertyName("value")]
            public List<GraphUser>? Value { get; set; }
        }

        private class GraphUser
        {
            public string? Id { get; set; }
            public string? DisplayName { get; set; }
            public string? UserPrincipalName { get; set; }
            public string? Mail { get; set; }
            public string? JobTitle { get; set; }
            public string? Department { get; set; }
        }
    }
}
