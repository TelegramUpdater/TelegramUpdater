// Ignore Spelling: Webpage

using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse.Abstraction;

/// <summary>
/// Abstract scoped update handler for all handlers with <see cref="Message"/> input.
/// </summary>
/// <param name="resolver"></param>
public abstract class AbstractMessageHandler(Func<Update, Message?> resolver)
    : AbstractScopedUpdateHandler<Message, MessageContainer>(resolver)
{
    /// <inheritdoc/>
    protected internal override MessageContainer ContainerBuilder(IUpdater updater, ShiningInfo<long, Update> shiningInfo)
        => new(updater, shiningInfo, ExtraData);

    #region Extension Methods
    /// <inheritdoc cref="Message.From"/>.
    protected User? From => ActualUpdate.From;

    /// <inheritdoc cref="Message.Chat"/>.
    protected Chat Chat => ActualUpdate.Chat;

    /// <inheritdoc cref="Message.ReplyToMessage"/>.
    protected Message? RepliedTo => ActualUpdate.ReplyToMessage;

    /// <inheritdoc cref="Message.MessageId"/>.
    protected int Id => ActualUpdate.MessageId;

    /// <summary>
    /// Check if the message is replied to another.
    /// </summary>
    protected bool IsReplied => RepliedTo is not null;

    /// <inheritdoc cref="TelegramBotClientExtensions.SendMessage(ITelegramBotClient, ChatId, string, ParseMode, ReplyParameters?, ReplyMarkup?, LinkPreviewOptions?, int?, IEnumerable{MessageEntity}?, bool, bool, string?, string?, bool, CancellationToken)"/>.
    /// <remarks>This methods sends a message to the <see cref="Message.Chat"/></remarks>
    protected async Task<Message> Response(
        string text,
        bool sendAsReply = false,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? messageEntities = default,
        bool? disableWebpagePreview = default,
        int? messageThreadId = default,
        bool disableNotification = default,
        ReplyMarkup? replyMarkup = default,
        bool protectContent = default,
        string? messageEffectId = default,
        string? businessConnectionId = default,
        bool allowPaidBroadcast = default,
        CancellationToken cancellationToken = default)
        => (await Container.Response(
            text: text,
            parseMode: parseMode,
            sendAsReply: sendAsReply,
            replyMarkup: replyMarkup,
            disableWebpagePreview: disableWebpagePreview,
            messageThreadId: messageThreadId,
            messageEntities: messageEntities,
            disableNotification: disableNotification,
            protectContent: protectContent,
            messageEffectId: messageEffectId,
            businessConnectionId: businessConnectionId,
            allowPaidBroadcast: allowPaidBroadcast,
            cancellationToken: cancellationToken).ConfigureAwait(false))
        .Update;

    /// <inheritdoc cref="TelegramBotClientExtensions.SendMessage(ITelegramBotClient, ChatId, string, ParseMode, ReplyParameters?, ReplyMarkup?, LinkPreviewOptions?, int?, IEnumerable{MessageEntity}?, bool, bool, string?, string?, bool, CancellationToken)"/>.
    protected async Task<Message> SendMessage(
        ChatId chatId,
        string text,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? entities = default,
        bool? disableWebPagePreview = default,
        int? messageThreadId = default,
        bool disableNotification = default,
        ReplyParameters? replyParameters = default,
        ReplyMarkup? replyMarkup = default,
        bool protectContent = default,
        string? messageEffectId = default,
        string? businessConnectionId = default,
        bool allowPaidBroadcast = default,
        CancellationToken cancellationToken = default)
        => await BotClient.SendMessage(
            chatId: chatId,
            text: text,
            parseMode: parseMode,
            replyParameters: replyParameters,
            replyMarkup: replyMarkup,
            linkPreviewOptions: disableWebPagePreview,
            messageThreadId: messageThreadId,
            entities: entities,
            disableNotification: disableNotification,
            protectContent: protectContent,
            messageEffectId: messageEffectId,
            businessConnectionId: businessConnectionId,
            allowPaidBroadcast: allowPaidBroadcast,
            cancellationToken: cancellationToken).ConfigureAwait(false);

    /// <inheritdoc cref="TelegramBotClientExtensions.DeleteMessage(ITelegramBotClient, ChatId, int, CancellationToken)"/>
    protected async Task Delete(CancellationToken cancellationToken = default)
    {
        await BotClient.DeleteMessage(Chat.Id, Id, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Asks a user to input an text message and waits for it.
    /// </summary>
    public async Task<string?> AwaitTextInput(
        TimeSpan timeOut,
        string text,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? entities = default,
        bool? disableWebPagePreview = default,
        bool disableNotification = default,
        bool protectContents = default,
        bool sendMessageAsReply = default,
        ReplyMarkup? replyMarkup = default,
        Func<CancellationToken, Task>? onTimeOut = default,
        Func<
            IUpdater,
            ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        if (text is not null)
            await Response(
                text,
                sendAsReply: sendMessageAsReply,
                parseMode: parseMode,
                messageEntities: entities,
                disableWebpagePreview: disableWebPagePreview,
                disableNotification: disableNotification,
                protectContent: protectContents,
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken).ConfigureAwait(false);

        var update = await AwaitMessage(
            FilterCutify.Text(), timeOut, onUnrelatedUpdate, cancellationToken).ConfigureAwait(false);
        if (update == null)
        {
            if (onTimeOut is not null)
                await onTimeOut(cancellationToken).ConfigureAwait(false);
            return null;
        }

        return update.Update.Text;
    }
    #endregion
}
