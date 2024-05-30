using System.Text.Json.Serialization;

namespace Seq.App.Discord.Data;

public readonly record struct WebhookMessageEmbed
{
    [JsonPropertyName("title")]
    public string Title { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("url")]
    public string Url { get; init; }

    [JsonPropertyName("color")]
    public int Color { get; init; }

    /// <summary>
    /// Time stored as string in the format: "yyyy-MM-ddTHH:mm:ss.fffZ"
    /// </summary>
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; init; }

}
