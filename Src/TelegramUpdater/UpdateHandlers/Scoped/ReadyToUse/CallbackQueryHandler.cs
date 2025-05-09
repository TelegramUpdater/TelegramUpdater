// Ignore Spelling: Webpage

using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.CallbackQuery"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class CallbackQueryHandler()
    : AbstractScopedUpdateHandler<CallbackQuery, CallbackQueryContainer>(x => x.CallbackQuery)
{
    /// <inheritdoc/>
    protected internal override CallbackQueryContainer ContainerBuilder(HandlerInput input)
        => new(input, ExtraData);

    #region Extension Methods
    /// <inheritdoc cref="CallbackQuery.From"/>.
    protected User From => ActualUpdate.From;

    /// <inheritdoc cref="CallbackQuery.Data"/>.
    protected string? Data => ActualUpdate.Data;

    /// <inheritdoc cref="CallbackQuery.Id"/>.
    protected string Id => ActualUpdate.Id;

    /// <inheritdoc cref="TelegramBotClientExtensions
    /// .AnswerCallbackQuery(ITelegramBotClient, string, string?, bool, string?, int?, CancellationToken)"/>.
    protected Task Answer(
        string? text = default,
        bool showAlert = default,
        string? url = default,
        int? cacheTime = default,
        CancellationToken cancellationToken = default)
        => Container.Answer(
            text: text,
            showAlert: showAlert,
            url: url,
            cacheTime: cacheTime,
            cancellationToken: cancellationToken);

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageText(ITelegramBotClient, ChatId, int, string, ParseMode, IEnumerable{MessageEntity}?, LinkPreviewOptions?, InlineKeyboardMarkup?, string?, CancellationToken)"/>
    public Task<IContainer<Message>?> Edit(
        string text,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? messageEntities = default,
        bool? disableWebpagePreview = default,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
        => Container.Edit(
            text: text,
            parseMode: parseMode,
            messageEntities: messageEntities,
            disableWebpagePreview: disableWebpagePreview,
            inlineKeyboardMarkup: inlineKeyboardMarkup,
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken);

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageLiveLocation(ITelegramBotClient, ChatId, int, double, double, int?, double?, int?, int?, InlineKeyboardMarkup?, string?, CancellationToken)"/>
    public Task<IContainer<Message>?> Edit(
        double latitude,
        double longitude,
        int? livePeriod = default,
        float? horizontalAccuracy = default,
        int? heading = default,
        int? proximityAlertRadius = default,
        InlineKeyboardMarkup? replyMarkup = default,
        string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
        => Container.Edit(
            latitude: latitude,
            longitude: longitude,
            livePeriod: livePeriod,
            horizontalAccuracy: horizontalAccuracy,
            heading: heading,
            proximityAlertRadius: proximityAlertRadius,
            replyMarkup: replyMarkup,
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken);

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageMedia(ITelegramBotClient, ChatId, int, InputMedia, InlineKeyboardMarkup?, string?, CancellationToken)"/>
    public Task<IContainer<Message>?> Edit(
        InputMedia inputMedia,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
        => Container.Edit(
            inputMedia: inputMedia,
            inlineKeyboardMarkup: inlineKeyboardMarkup,
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken);

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageCaption(ITelegramBotClient, string, string?, ParseMode, IEnumerable{MessageEntity}?, bool, InlineKeyboardMarkup?, string?, CancellationToken)"/>
    public Task<IContainer<Message>?> EditCaption(
        string caption,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? captionEntities = default,
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
        => Container.EditCaption(
            caption: caption,
            parseMode: parseMode,
            captionEntities: captionEntities,
            inlineKeyboardMarkup: inlineKeyboardMarkup,
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken);

    /// <inheritdoc cref="TelegramBotClientExtensions.EditMessageReplyMarkup(ITelegramBotClient, string, InlineKeyboardMarkup?, string?, CancellationToken)"/>
    public Task<IContainer<Message>?> Edit(
        InlineKeyboardMarkup? inlineKeyboardMarkup = default,
        string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
        => Container.Edit(
            inlineKeyboardMarkup: inlineKeyboardMarkup,
            businessConnectionId: businessConnectionId,
            cancellationToken: cancellationToken);
    #endregion
}
