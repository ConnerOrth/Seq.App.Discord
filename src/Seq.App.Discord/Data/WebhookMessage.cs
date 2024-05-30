using System.Text.Json.Serialization;

namespace Seq.App.Discord.Data;

/// <summary>
/// see: https://birdie0.github.io/discord-webhooks-guide/discord_webhook.html
/// </summary>
public readonly record struct WebhookMessage
{
    [JsonPropertyName("username")]
    public string UserName { get; init; }

    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; init; }
    
    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("embeds")]
    public WebhookMessageEmbed[] Embeds { get; init; }
}
