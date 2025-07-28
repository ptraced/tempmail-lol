using System.Text;
using System.Text.Json;
using TempMail.Lol.Models;

namespace TempMail.Lol;

/// <summary>
/// Client for interacting with the TempMail.lol temporary email service.
/// </summary>
public class TempMailClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    private const string BaseUrl = "https://api.tempmail.lol/v2";

    /// <summary>
    /// Initializes a new instance of the TempMailClient.
    /// </summary>
    /// <param name="apiKey">Optional API key for TempMail Plus/Ultra accounts.</param>
    /// <param name="httpClient">Optional HttpClient instance. If not provided, a new one will be created.</param>
    public TempMailClient(string? apiKey = null, HttpClient? httpClient = null)
    {
        _apiKey = apiKey;
        _httpClient = httpClient ?? new HttpClient();
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        }
    }

    /// <summary>
    /// Creates a new temporary email inbox.
    /// </summary>
    /// <param name="options">Options for creating the inbox.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A new temporary email inbox.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="JsonException">Thrown when the response cannot be parsed.</exception>
    public async Task<Inbox> CreateInboxAsync(CreateInboxOptions? options = null, CancellationToken cancellationToken = default)
    {
        var requestBody = options ?? new CreateInboxOptions();
        var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(BaseUrl + "/inbox/create", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        Console.WriteLine(responseJson);
        var inbox = JsonSerializer.Deserialize<Inbox>(responseJson, _jsonOptions);
        
        return inbox ?? throw new JsonException("Failed to deserialize inbox response");
    }

    /// <summary>
    /// Checks the inbox for new emails.
    /// </summary>
    /// <param name="token">The inbox token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of emails in the inbox, or null if the inbox has expired.</returns>
    /// <exception cref="ArgumentNullException">Thrown when token is null or empty.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="JsonException">Thrown when the response cannot be parsed.</exception>
    public async Task<IReadOnlyList<Email>?> CheckInboxAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        try
        {
            var response = await _httpClient.GetAsync(BaseUrl + $"/inbox?token={token}", cancellationToken);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine(responseJson);
            var emails = JsonSerializer.Deserialize<Email[]>(responseJson, _jsonOptions);
            
            return emails ?? Array.Empty<Email>();
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    /// <summary>
    /// Checks the inbox for new emails using an Inbox object.
    /// </summary>
    /// <param name="inbox">The inbox object containing the token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of emails in the inbox, or null if the inbox has expired.</returns>
    /// <exception cref="ArgumentNullException">Thrown when inbox is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="JsonException">Thrown when the response cannot be parsed.</exception>
    public async Task<IReadOnlyList<Email>?> CheckInboxAsync(Inbox inbox, CancellationToken cancellationToken = default)
    {
        if (inbox == null)
            throw new ArgumentNullException(nameof(inbox));

        return await CheckInboxAsync(inbox.Token, cancellationToken);
    }

    /// <summary>
    /// Sets a webhook URL to receive notifications when emails are received.
    /// Requires TempMail Ultra subscription and Custom Domains setup.
    /// </summary>
    /// <param name="webhookUrl">The webhook URL to set.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentNullException">Thrown when webhookUrl is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no API key is provided.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    public async Task SetWebhookAsync(string webhookUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(webhookUrl))
            throw new ArgumentNullException(nameof(webhookUrl));

        if (string.IsNullOrEmpty(_apiKey))
            throw new InvalidOperationException("API key is required for webhook operations");

        var requestBody = new { url = webhookUrl };
        var json = JsonSerializer.Serialize(requestBody, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(BaseUrl + "/webhook", content, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Removes the currently set webhook.
    /// Requires TempMail Ultra subscription and Custom Domains setup.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="InvalidOperationException">Thrown when no API key is provided.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    public async Task RemoveWebhookAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_apiKey))
            throw new InvalidOperationException("API key is required for webhook operations");

        var response = await _httpClient.DeleteAsync(BaseUrl + "/webhook", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Disposes the HttpClient if it was created by this instance.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
} 