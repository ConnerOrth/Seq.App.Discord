namespace Seq.App.Discord;

internal readonly record struct Settings(
    string SeqBaseUrl,
    string DiscordWebhookUrl,
    string? AvatarUrl,
    string Username,
    string? Content,
    string? TitlePropertyName);
