using System.Text.Json.Serialization;

namespace TempMail.Lol.Models;

/// <summary>
/// Represents a temporary email inbox.
/// </summary>
public class Inbox
{
    /// <summary>
    /// The email address of the inbox.
    /// </summary>
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// The token used to access the inbox and retrieve emails.
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
} 