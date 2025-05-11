// Ignore Spelling: Inline

using Telegram.Bot.Types.InlineQueryResults;

namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.Message"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class InlineQueryControllerHandler()
    : DefaultControllerHandler<InlineQuery>(x => x.InlineQuery)
{
    #region Extension Methods
    /// <inheritdoc cref="InlineQuery.From"/>.
    protected User From => ActualUpdate.From;

    /// <inheritdoc cref="InlineQuery.Id"/>.
    protected string Id => ActualUpdate.Id;

    /// <inheritdoc cref="InlineQuery.Query"/>.
    protected string Query => ActualUpdate.Query;

    /// <inheritdoc cref="InlineQuery.ChatType"/>.
    protected ChatType? ChatType => ActualUpdate.ChatType;

    /// <inheritdoc cref="TelegramBotClientExtensions.AnswerInlineQuery(ITelegramBotClient, string, IEnumerable{InlineQueryResult}, int?, bool, string?, InlineQueryResultsButton?, CancellationToken)"/>.
    public async Task Answer(
        IEnumerable<InlineQueryResult> results,
        int? cacheTime = null,
        bool isPersonal = default,
        string? nextOffset = null,
        InlineQueryResultsButton? button = null,
        CancellationToken cancellationToken = default)
        => await BotClient.AnswerInlineQuery(
            Id,
            results,
            cacheTime,
            isPersonal,
            nextOffset,
            button,
            cancellationToken).ConfigureAwait(false);
    #endregion
}
