using Telegram.Bot.Types.InlineQueryResults;

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.Message"/>.
/// </summary>
public abstract class InlineQueryHandler
    : AnyHandler<InlineQuery>
{
    /// <summary>
    /// Set handling priority of this handler.
    /// </summary>
    /// <param name="group">Handling priority group, The lower the sooner to process.</param>
    protected InlineQueryHandler(int group = default)
        : base(x => x.InlineQuery, group)
    {
    }


    #region Extension Methods
    /// <inheritdoc cref="InlineQuery.From"/>.
    protected User From => ActualUpdate.From;

    /// <inheritdoc cref="InlineQuery.Id"/>.
    protected string Id => ActualUpdate.Id;

    /// <inheritdoc cref="InlineQuery.Query"/>.
    protected string Query => ActualUpdate.Query;

    /// <inheritdoc cref="InlineQuery.ChatType"/>.
    protected ChatType? ChatType => ActualUpdate.ChatType;

    /// <inheritdoc cref="TelegramBotClientExtensions.AnswerInlineQueryAsync(
    /// ITelegramBotClient, string, IEnumerable{InlineQueryResult},
    /// int?, bool?, string?, string?, string?, CancellationToken)"/>.
    public async Task AnswerAsync(
        IEnumerable<InlineQueryResult> results,
        int? cacheTime = null,
        bool? isPersonal = null,
        string? nextOffset = null,
        string? switchPmText = null,
        string? switchPmParameter = null,
        CancellationToken cancellationToken = default)
    {
        await BotClient.AnswerInlineQueryAsync(
            Id, results, cacheTime, isPersonal, nextOffset,
            switchPmText, switchPmParameter, cancellationToken);
    }

    /// <summary>
    /// Answer an inline query with a switch private message.
    /// </summary>
    /// <param name="switchPmText">
    /// If passed, clients will display a button with specified text that switches the user to a private chat
    /// with the bot and sends the bot a start message with the parameter <paramref name="switchPmParameter"/>
    /// </param>
    /// <param name="switchPmParameter">
    /// <see href="https://core.telegram.org/bots#deep-linking">Deep-linking</see> parameter for the <c>/start</c>
    /// message sent to the bot when user presses the switch button. 1-64 characters, only <c>A-Z</c>, <c>a-z</c>,
    /// <c>0-9</c>, <c>_</c> and <c>-</c> are allowed
    /// <para>
    /// <i>Example</i>: An inline bot that sends YouTube videos can ask the user to connect the bot to their
    /// YouTube account to adapt search results accordingly. To do this, it displays a 'Connect your YouTube
    /// account' button above the results, or even before showing any. The user presses the button, switches
    /// to a private chat with the bot and, in doing so, passes a start parameter that instructs the bot to
    /// return an oauth link. Once done, the bot can offer a
    /// <see cref="Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.SwitchInlineQuery"/> button so that the user can
    /// easily return to the chat where they wanted to use the bot's inline capabilities
    /// </para>
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used by other objects or threads to receive notice of cancellation
    /// </param>
    /// <returns></returns>
    public async Task SwitchPmAsync(
        string? switchPmText = null,
        string? switchPmParameter = null,
        CancellationToken cancellationToken = default)
    {
        await BotClient.AnswerInlineQueryAsync(
            Id, results: Array.Empty<InlineQueryResult>(),
            switchPmText: switchPmText,
            switchPmParameter: switchPmParameter,
            cancellationToken: cancellationToken);
    }
    #endregion
}
