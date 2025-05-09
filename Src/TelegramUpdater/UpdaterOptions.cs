// Ignore Spelling: Webhook

using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace TelegramUpdater;

/// <summary>
/// Sets options for <see cref="IUpdater"/>.
/// </summary>
public class UpdaterOptions
{
    /// <summary>
    /// Options section name.
    /// </summary>
    public const string Updater = "TelegramUpdater";

    /// <param name="botToken">The bot token. Please include this if you're not going to pass an <see cref="ITelegramBotClient"/>.</param>
    /// <param name="maxDegreeOfParallelism">
    /// Maximum number of allowed concurrent update handling tasks.
    /// </param>
    /// <param name="logger">If you want to use your own logger.</param>
    /// <param name="cancellationToken">
    /// Default token to be used in Start method.
    /// </param>
    /// <param name="flushUpdatesQueue">Old updates will gone.</param>
    /// <param name="allowedUpdates">Allowed updates.</param>
    /// <param name="switchChatId">
    /// By enabling this option, the updater will try to resolve
    /// chat id from update and use it
    /// as queue keys. if there's no user id available.
    /// </param>
    public UpdaterOptions(
        string? botToken = default,
        int? maxDegreeOfParallelism = default,
        ILogger<IUpdater>? logger = default,
        bool flushUpdatesQueue = default,
        UpdateType[]? allowedUpdates = default,
        bool switchChatId = true,
        CancellationToken cancellationToken = default)
    {
        BotToken = botToken;
        MaxDegreeOfParallelism = maxDegreeOfParallelism;
        FlushUpdatesQueue = flushUpdatesQueue;
        AllowedUpdates = allowedUpdates;
        SwitchChatId = switchChatId;
        Logger = logger;
        CancellationToken = cancellationToken;
    }

    /// <summary>
    /// Create a new instance of <see cref="UpdaterOptions"/>
    /// </summary>
    public UpdaterOptions() { }

    /// <summary>
    /// Bot token to be used.
    /// </summary>
    public string? BotToken { get; init; }

    /// <summary>
    /// Maximum number of allowed concurrent update handling tasks.
    /// </summary>
    public int? MaxDegreeOfParallelism { get; init; }

    /// <summary>
    /// Old updates will gone.
    /// </summary>
    public bool FlushUpdatesQueue { get; init; }

    /// <summary>
    /// Allowed updates.
    /// </summary>
    public UpdateType[]? AllowedUpdates { get; set; }

    /// <summary>
    /// By enabling this option, the updater will try to
    /// resolve chat id from update and use it
    /// as queue keys. if there's no user id available.
    /// </summary>
    /// <remarks>
    /// Normally, the updater tries to resolve a sender id ( user )
    /// from an update and use it as
    /// queue keys.
    /// </remarks>
    public bool SwitchChatId { get; init; }

    /// <summary>
    /// Webhook url in case of using webhook.
    /// </summary>
    public Uri? BotWebhookUrl { get; init; }

    /// <summary>
    /// Secret token in case of using webhook.
    /// </summary>
    public string? SecretToken { get; init; }

    /// <summary>
    /// If you want to use your own logger.
    /// </summary>
    [JsonIgnore]
    public ILogger<IUpdater>? Logger { get; }

    // TODO: Remove this
    /// <summary>
    /// Default token to be used in Start method.
    /// </summary>
    [JsonIgnore]
    public CancellationToken CancellationToken { get; }
}
