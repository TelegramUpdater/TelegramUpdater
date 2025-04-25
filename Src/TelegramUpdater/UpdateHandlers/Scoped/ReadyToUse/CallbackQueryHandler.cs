namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.CallbackQuery"/>.
/// </summary>
public abstract class CallbackQueryHandler
    : AnyHandler<CallbackQuery>
{
    /// <summary>
    /// Set handling priority of this handler.
    /// </summary>
    /// <param name="group">Handling priority group, The lower the sooner to process.</param>
    protected CallbackQueryHandler(int group = 0)
        : base(x => x.CallbackQuery, group)
    {
    }


    #region Extension Methods
    /// <inheritdoc cref="CallbackQuery.From"/>.
    protected User From => ActualUpdate.From;

    /// <inheritdoc cref="CallbackQuery.Data"/>.
    protected string? Data => ActualUpdate.Data;

    /// <inheritdoc cref="CallbackQuery.Id"/>.
    protected string Id => ActualUpdate.Id;

    /// <inheritdoc cref="TelegramBotClientExtensions
    /// .AnswerCallbackQuery(ITelegramBotClient, string, string?, bool, string?, int?, CancellationToken)"/>.
    protected async Task AnswerAsync(
        string? text = default,
        bool showAlert = default,
        string? url = default,
        int? cacheTime = default,
        CancellationToken cancellationToken = default)
    {
        await BotClient.AnswerCallbackQuery(
            Id, text, showAlert, url, cacheTime, cancellationToken);
    }
    #endregion
}
