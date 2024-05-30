using Seq.App.Discord.Data;
using Seq.Apps;
using Seq.Apps.LogEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Seq.App.Discord;

[SeqApp("Discord", Description = "Sends log events to Discord.")]
public class DiscordApp : SeqApp, ISubscribeToAsync<LogEventData>
{
    private static readonly IDictionary<LogEventLevel, int> levelColorMap = new Dictionary<LogEventLevel, int>
    {
        {LogEventLevel.Verbose, 8421504},
        {LogEventLevel.Debug, 8421504},
        {LogEventLevel.Information, 32768},
        {LogEventLevel.Warning, 16776960},
        {LogEventLevel.Error, 16711680},
        {LogEventLevel.Fatal, 16711680},
    };

    private readonly HttpClient client;

    private static Settings settings = new Settings();

    public DiscordApp() : this(new HttpClient()) { }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal DiscordApp(HttpClient client)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        this.client = client;
        this.client.Timeout = TimeSpan.FromSeconds(10);
    }


    [SeqAppSetting(DisplayName = "Seq Base URL",
        HelpText = "Used for generating perma links to events in Discord messages.")]
    public string SeqBaseUrl { get; set; }

    [SeqAppSetting(DisplayName = "Discord Webhook URL",
        HelpText = "This has to be generated through discord application for a channel")]
    public string DiscordWebhookUrl { get; set; }

    [SeqAppSetting(DisplayName = "Title property name",
        HelpText = "Name of an event property to be used in title of a message (default: loglevel of message)",
        IsOptional = true)]
    public string TitlePropertyName { get; set; }

    [SeqAppSetting(DisplayName = "Bot name",
        HelpText = "Notifier bot name (default: Seq notifier)",
        IsOptional = true)]
    public string? NotifierBotName { get; set; }

    [SeqAppSetting(DisplayName = "Discord Avatar URL",
        HelpText = "Url to any image that fits Discord avatar requirements",
        IsOptional = true)]
    public string? AvatarUrl { get; set; }

    [SeqAppSetting(DisplayName = "Discord Role Ids",
        HelpText = "Discord Role IDs to mention in the content of the message, ',' seperated list.",
        IsOptional = true)]
    public string? RolesToMention { get; set; }

    [SeqAppSetting(InputType = SettingInputType.Checkbox, IsOptional = true, DisplayName = "Extended Error Diagnostics",
            HelpText = "Whether or not to include outbound request bodies, URLs, etc., and response bodies when requests fail.")]
    public bool ExtendedErrorDiagnostics { get; set; }

    protected override void OnAttached()
    {
        settings = new Settings()
        {
            SeqBaseUrl = SeqBaseUrl ?? throw new InvalidOperationException("The `SeqBaseUrl` setting is required."),
            DiscordWebhookUrl = DiscordWebhookUrl ?? throw new InvalidOperationException("The `DiscordWebhookUrl` setting is required."),
            AvatarUrl = AvatarUrl,
            Username = !string.IsNullOrWhiteSpace(NotifierBotName) ? NotifierBotName : "Seq notifier",
            Content = !string.IsNullOrWhiteSpace(RolesToMention)
                ? RolesToMention.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => $"<@&{f}>")
                    .Aggregate((s, x) => s + x)
                : null
        };
    }

    public async Task OnAsync(Event<LogEventData> @event)
    {
        var webhookMessage = CreateMessage(@event, settings);

        var response = await client.PostAsJsonAsync(settings.DiscordWebhookUrl, webhookMessage);
        if (response.IsSuccessStatusCode) return;

        var log = Log;
        if (ExtendedErrorDiagnostics)
        {
            log = log
                .ForContext("RequestUrl", response.RequestMessage?.RequestUri)
                .ForContext("ResponseBody", await response.Content.ReadAsStringAsync());
        }

        log.Error("Outbound HTTP request failed with status code {StatusCode}", response.StatusCode);
    }

    private static string GetTitle(LogEventData eventData, string? titlePropertyName)
    {
        if (string.IsNullOrWhiteSpace(titlePropertyName) || !eventData.Properties.ContainsKey(titlePropertyName))
        {
            return eventData.Level.ToString();
        }

        return eventData.Properties[titlePropertyName].ToString() + " - " + eventData.Level.ToString();
    }

    private static WebhookMessage CreateMessage(Event<LogEventData> @event, Settings settings)
    {
        return new WebhookMessage
        {
            UserName = settings.Username,
            AvatarUrl = settings.AvatarUrl,
            Content = settings.Content,
            Embeds = new WebhookMessageEmbed[]
            {
                new WebhookMessageEmbed()
                {
                    Title = GetTitle(@event.Data, settings.TitlePropertyName),
                    Url = string.Format("{0}/#/events?filter=@Id%20%3D%3D%20%22{1}%22&show=expanded", settings.SeqBaseUrl, @event.Id ),
                    Description = @event.Data.RenderedMessage,
                    Color = levelColorMap[@event.Data.Level],
                    Timestamp = @event.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                }
            }
        };
    }
}
