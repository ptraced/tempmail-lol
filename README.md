# TempMail.Lol - C# Client Library

A C# wrapper for the [TempMail.lol](https://tempmail.lol) temporary email service API. Create temporary email addresses and retrieve emails programmatically.

[![NuGet](https://img.shields.io/nuget/v/TempMail.Lol.svg)](https://www.nuget.org/packages/TempMail.Lol)

## Features

- ✅ Create temporary email inboxes
- ✅ Retrieve emails from inboxes
- ✅ Support for custom domains (Plus/Ultra subscribers)
- ✅ Webhook support (Ultra subscribers)
- ✅ Async/await support
- ✅ Built-in JSON serialization
- ✅ Comprehensive error handling
- ✅ XML documentation

## Installation

Install the package via NuGet Package Manager:

```bash
dotnet add package TempMail.Lol
```

Or via Package Manager Console:

```powershell
Install-Package TempMail.Lol
```

## Quick Start

### Basic Usage (Free)

```csharp
using TempMail.Lol;
using TempMail.Lol.Models;

// Create a client (no API key needed for basic features)
using var client = new TempMailClient();

// Create a temporary inbox
var inbox = await client.CreateInboxAsync();
Console.WriteLine($"Created inbox: {inbox.Address}");

// Check for emails (returns null if inbox expired)
var emails = await client.CheckInboxAsync(inbox);
if (emails != null)
{
    Console.WriteLine($"Found {emails.Count} emails");
    foreach (var email in emails)
    {
        Console.WriteLine($"From: {email.From}");
        Console.WriteLine($"Subject: {email.Subject}");
        Console.WriteLine($"Body: {email.Body}");
        Console.WriteLine($"Date: {email.DateTime}");
        Console.WriteLine("---");
    }
}
else
{
    Console.WriteLine("Inbox has expired or doesn't exist");
}
```

### Advanced Usage with Options

```csharp
using TempMail.Lol;
using TempMail.Lol.Models;

using var client = new TempMailClient();

// Create inbox with custom options
var options = new CreateInboxOptions
{
    Community = false,      // Use private domain (if available)
    Domain = "example.com", // Custom domain (Plus/Ultra only)
    Prefix = "myname"       // Custom prefix: myname@example.com
};

var inbox = await client.CreateInboxAsync(options);
Console.WriteLine($"Created custom inbox: {inbox.Address}");
```

### Premium Features (Plus/Ultra Subscribers)

```csharp
using TempMail.Lol;

// Initialize with API key for premium features
using var client = new TempMailClient("your-api-key");

// Set up webhook (Ultra subscribers only)
await client.SetWebhookAsync("https://yourdomain.com/webhook");
Console.WriteLine("Webhook configured");

// Remove webhook
await client.RemoveWebhookAsync();
Console.WriteLine("Webhook removed");
```

## API Reference

### TempMailClient

#### Constructor

```csharp
public TempMailClient(string? apiKey = null, HttpClient? httpClient = null)
```

- `apiKey`: Optional API key for TempMail Plus/Ultra accounts
- `httpClient`: Optional HttpClient instance (useful for dependency injection)

#### Methods

##### CreateInboxAsync

```csharp
public async Task<Inbox> CreateInboxAsync(CreateInboxOptions? options = null, CancellationToken cancellationToken = default)
```

Creates a new temporary email inbox.

**Parameters:**
- `options`: Optional inbox creation options
- `cancellationToken`: Cancellation token

**Returns:** `Inbox` object containing the email address and token.

##### CheckInboxAsync

```csharp
public async Task<IReadOnlyList<Email>?> CheckInboxAsync(string token, CancellationToken cancellationToken = default)
public async Task<IReadOnlyList<Email>?> CheckInboxAsync(Inbox inbox, CancellationToken cancellationToken = default)
```

Checks the inbox for emails.

**Parameters:**
- `token` or `inbox`: The inbox token or Inbox object
- `cancellationToken`: Cancellation token

**Returns:** List of emails, or `null` if the inbox has expired.

##### SetWebhookAsync (Ultra only)

```csharp
public async Task SetWebhookAsync(string webhookUrl, CancellationToken cancellationToken = default)
```

Sets a webhook URL to receive email notifications.

##### RemoveWebhookAsync (Ultra only)

```csharp
public async Task RemoveWebhookAsync(CancellationToken cancellationToken = default)
```

Removes the currently configured webhook.

### Models

#### Inbox

```csharp
public class Inbox
{
    public string Address { get; set; }  // The email address
    public string Token { get; set; }    // Token for accessing the inbox
}
```

#### Email

```csharp
public class Email
{
    public string From { get; set; }      // Sender email address
    public string To { get; set; }        // Recipient email address
    public string Subject { get; set; }   // Email subject
    public string Body { get; set; }      // Plain text body
    public string? Html { get; set; }     // HTML content (if available)
    public long Date { get; set; }        // Unix timestamp in milliseconds
    public string Ip { get; set; }        // Sender's IP address
    public DateTime DateTime { get; }     // Converted DateTime object
}
```

#### CreateInboxOptions

```csharp
public class CreateInboxOptions
{
    public bool Community { get; set; } = true;     // Use community domain
    public string? Domain { get; set; }             // Custom domain (Plus/Ultra)
    public string? Prefix { get; set; }             // Custom username prefix
}
```

## Error Handling

The library throws standard .NET exceptions:

- `HttpRequestException`: API request failures
- `JsonException`: Response parsing errors
- `ArgumentNullException`: Invalid parameters
- `InvalidOperationException`: Missing API key for premium features

```csharp
try
{
    var inbox = await client.CreateInboxAsync();
    var emails = await client.CheckInboxAsync(inbox);
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"API request failed: {ex.Message}");
}
catch (JsonException ex)
{
    Console.WriteLine($"Failed to parse response: {ex.Message}");
}
```

## Dependency Injection

The library works well with .NET's dependency injection:

```csharp
// Program.cs or Startup.cs
services.AddHttpClient<TempMailClient>();
services.AddSingleton(provider => 
    new TempMailClient("your-api-key", provider.GetService<HttpClient>()));
```

## Subscription Tiers

- **Free**: Basic inbox creation and email retrieval (expires after 1 hour)
- **Plus**: Extended expiration (10 hours) and custom domains
- **Ultra**: Extended expiration (30 hours), custom domains, and webhooks

For subscription details, visit: https://tempmail.lol/account

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Disclaimer

This library is not officially affiliated with TempMail.lol. It's a community-driven wrapper for their API.

## Related Links

- [TempMail.lol Website](https://tempmail.lol)
- [API Documentation](https://tempmail.lol/en/api)
- [Official JavaScript Library](https://github.com/tempmail-lol/api-javascript) 