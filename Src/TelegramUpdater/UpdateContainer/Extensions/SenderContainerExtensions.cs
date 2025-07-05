// Ignore Spelling: Webpage

using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.UpdateContainer.Tags;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TelegramUpdater.UpdateContainer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// A set of extension methods for types implement <see cref="IBaseContainer"/> and <see cref="ISenderIdExtractable"/>. 
/// </summary>
public static class SenderContainerExtensions
{
    /// <inheritdoc cref="TelegramBotClientExtensions.SendMessage(ITelegramBotClient, ChatId, string, ParseMode, ReplyParameters?, ReplyMarkup?, LinkPreviewOptions?, int?, IEnumerable{MessageEntity}?, bool, bool, string?, string?, bool, CancellationToken)" />
    public static async Task<IBaseContainer<Message>> SendMessage<C>(
        this C simpleContext,
        string text,
        ParseMode parseMode = default,
        ReplyParameters? replyParameters = default,
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
        where C : IBaseContainer, ISenderIdExtractable
        => await simpleContext.BotClient.SendMessage(
            chatId: simpleContext.GetEnsuredSenderId(),
            text: text,
            parseMode: parseMode,
            replyParameters: replyParameters,
            replyMarkup: replyMarkup,
            linkPreviewOptions: disableWebpagePreview,
            messageThreadId: messageThreadId,
            entities: messageEntities,
            disableNotification: disableNotification,
            protectContent: protectContent,
            messageEffectId: messageEffectId,
            businessConnectionId: businessConnectionId,
            allowPaidBroadcast: allowPaidBroadcast,
            cancellationToken: cancellationToken)
        .WrapMessageAsync(simpleContext.Updater).ConfigureAwait(false);
}
