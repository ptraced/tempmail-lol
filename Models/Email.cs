using System.Text.Json.Serialization;

namespace TempMail.Lol.Models;

public class Emails
{
    public List<Email> emails { get; set; }
    public bool expired { get; set; }
}


/// <summary>
/// Represents an email message received in a temporary email inbox.
/// </summary>
public class Email
{
    /// <summary>
    /// The sender's email address.
    /// </summary>
    [JsonPropertyName("from")]
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// The recipient's email address.
    /// </summary>
    [JsonPropertyName("to")]
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// The subject line of the email.
    /// </summary>
    [JsonPropertyName("subject")]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// The plain text body of the email.
    /// </summary>
    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// The HTML content of the email (if available).
    /// </summary>
    [JsonPropertyName("html")]
    public string? Html { get; set; }

    /// <summary>
    /// The date the email was received (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("date")]
    public long Date { get; set; }

    /// <summary>
    /// The IP address of the sender.
    /// </summary>
    [JsonPropertyName("ip")]
    public string Ip { get; set; } = string.Empty;

    /// <summary>
    /// Gets the date as a DateTime object.
    /// </summary>
    [JsonIgnore]
    public DateTime DateTime => DateTimeOffset.FromUnixTimeMilliseconds(Date).DateTime;
} 