using System.Text.Json.Serialization;

namespace TempMail.Lol.Models;

/// <summary>
/// Options for creating a temporary email inbox.
/// </summary>
public class CreateInboxOptions
{
    /// <summary>
    /// Whether to create a community address. Default is true.
    /// </summary>
    [JsonPropertyName("community")]
    public bool Community { get; set; } = true;

    /// <summary>
    /// The domain to use for the email address. If not specified, a random domain will be used.
    /// </summary>
    [JsonPropertyName("domain")]
    public string? Domain { get; set; }

    /// <summary>
    /// The prefix (username part) of the email address. If not specified, a random prefix will be generated.
    /// </summary>
    [JsonPropertyName("prefix")]
    public string? Prefix { get; set; }
} 