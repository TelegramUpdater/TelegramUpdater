namespace TelegramUpdater.Helpers;

/// <summary>The type of <see cref="Update"/> attached with <see cref="FlagsAttribute"/>.</summary>
[Flags]
public enum UpdateTypeFlags
{
    /// <summary><see cref="Update"/> type is unknown</summary>
    Unknown = 1,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.Message"/></summary>
    Message = 2,
    /// <summary>The <see cref="Update"/> contains an <see cref="Update.InlineQuery"/></summary>
    InlineQuery = 4,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.ChosenInlineResult"/></summary>
    ChosenInlineResult = 8,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.CallbackQuery"/></summary>
    CallbackQuery = 16,
    /// <summary>The <see cref="Update"/> contains an <see cref="Update.EditedMessage"/></summary>
    EditedMessage = 32,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.ChannelPost"/></summary>
    ChannelPost = 64,
    /// <summary>The <see cref="Update"/> contains an <see cref="Update.EditedChannelPost"/></summary>
    EditedChannelPost = 128,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.ShippingQuery"/></summary>
    ShippingQuery = 256,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.PreCheckoutQuery"/></summary>
    PreCheckoutQuery = 512,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.Poll"/></summary>
    Poll = 1024,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.PollAnswer"/></summary>
    PollAnswer = 2048,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.MyChatMember"/></summary>
    MyChatMember = 4096,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.ChatMember"/></summary>
    ChatMember = 8192,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.ChatJoinRequest"/></summary>
    ChatJoinRequest = 16384,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.MessageReaction"/></summary>
    MessageReaction = 32768,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.MessageReactionCount"/></summary>
    MessageReactionCount = 65536,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.ChatBoost"/></summary>
    ChatBoost = 131072,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.RemovedChatBoost"/></summary>
    RemovedChatBoost = 262144,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.BusinessConnection"/></summary>
    BusinessConnection = 524288,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.BusinessMessage"/></summary>
    BusinessMessage = 1048576,
    /// <summary>The <see cref="Update"/> contains an <see cref="Update.EditedBusinessMessage"/></summary>
    EditedBusinessMessage = 2097152,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.DeletedBusinessMessages"/></summary>
    DeletedBusinessMessages = 4194304,
    /// <summary>The <see cref="Update"/> contains a <see cref="Update.PurchasedPaidMedia"/></summary>
    PurchasedPaidMedia = 8388608,

    /// <summary>
    /// Update types where actual type of update is a <see cref="Telegram.Bot.Types.Message"/>.
    /// </summary>
    Messages = Message | EditedMessage | ChannelPost | EditedChannelPost | BusinessMessage | EditedBusinessMessage,

    /// <summary>
    /// Update types where actual type of update is a <see cref="ChatMemberUpdated"/>.
    /// </summary>
    ChatMemberUpdates = ChatMember | MyChatMember,
}

/// <summary>
/// Extensions for <see cref="UpdateTypeFlags"/>.
/// </summary>
public static class UpdateTypeFlagsExtensions
{
    /// <summary>
    /// Convert <see cref="UpdateTypeFlags"/> to <see cref="UpdateType"/>.
    /// </summary>
    /// <returns></returns>
    public static UpdateType RemoveFlag(this UpdateTypeFlags flags)
        => flags switch
        {
            UpdateTypeFlags.Unknown => UpdateType.Unknown,
            UpdateTypeFlags.Message => UpdateType.Message,
            UpdateTypeFlags.InlineQuery => UpdateType.InlineQuery,
            UpdateTypeFlags.ChosenInlineResult => UpdateType.ChosenInlineResult,
            UpdateTypeFlags.CallbackQuery => UpdateType.CallbackQuery,
            UpdateTypeFlags.EditedMessage => UpdateType.EditedMessage,
            UpdateTypeFlags.ChannelPost => UpdateType.ChannelPost,
            UpdateTypeFlags.EditedChannelPost => UpdateType.EditedChannelPost,
            UpdateTypeFlags.ShippingQuery => UpdateType.ShippingQuery,
            UpdateTypeFlags.PreCheckoutQuery => UpdateType.PreCheckoutQuery,
            UpdateTypeFlags.Poll => UpdateType.Poll,
            UpdateTypeFlags.PollAnswer => UpdateType.PollAnswer,
            UpdateTypeFlags.MyChatMember => UpdateType.MyChatMember,
            UpdateTypeFlags.ChatMember => UpdateType.ChatMember,
            UpdateTypeFlags.ChatJoinRequest => UpdateType.ChatJoinRequest,
            UpdateTypeFlags.MessageReaction => UpdateType.MessageReaction,
            UpdateTypeFlags.MessageReactionCount => UpdateType.MessageReactionCount,
            UpdateTypeFlags.ChatBoost => UpdateType.ChatBoost,
            UpdateTypeFlags.RemovedChatBoost => UpdateType.RemovedChatBoost,
            UpdateTypeFlags.BusinessConnection => UpdateType.BusinessConnection,
            UpdateTypeFlags.BusinessMessage => UpdateType.BusinessMessage,
            UpdateTypeFlags.EditedBusinessMessage => UpdateType.EditedBusinessMessage,
            UpdateTypeFlags.DeletedBusinessMessages => UpdateType.DeletedBusinessMessages,
            UpdateTypeFlags.PurchasedPaidMedia => UpdateType.PurchasedPaidMedia,
            _ => UpdateType.Unknown,
        };

    /// <summary>
    /// Convert <see cref="UpdateType"/> to <see cref="UpdateTypeFlags"/>.
    /// </summary>
    /// <returns></returns>
    public static UpdateTypeFlags AsFlag(this UpdateType updateType)
        => updateType switch
        {
            UpdateType.Unknown => UpdateTypeFlags.Unknown,
            UpdateType.Message => UpdateTypeFlags.Message,
            UpdateType.InlineQuery => UpdateTypeFlags.InlineQuery,
            UpdateType.ChosenInlineResult => UpdateTypeFlags.ChosenInlineResult,
            UpdateType.CallbackQuery => UpdateTypeFlags.CallbackQuery,
            UpdateType.EditedMessage => UpdateTypeFlags.EditedMessage,
            UpdateType.ChannelPost => UpdateTypeFlags.ChannelPost,
            UpdateType.EditedChannelPost => UpdateTypeFlags.EditedChannelPost,
            UpdateType.ShippingQuery => UpdateTypeFlags.ShippingQuery,
            UpdateType.PreCheckoutQuery => UpdateTypeFlags.PreCheckoutQuery,
            UpdateType.Poll => UpdateTypeFlags.Poll,
            UpdateType.PollAnswer => UpdateTypeFlags.PollAnswer,
            UpdateType.MyChatMember => UpdateTypeFlags.MyChatMember,
            UpdateType.ChatMember => UpdateTypeFlags.ChatMember,
            UpdateType.ChatJoinRequest => UpdateTypeFlags.ChatJoinRequest,
            UpdateType.MessageReaction => UpdateTypeFlags.MessageReaction,
            UpdateType.MessageReactionCount => UpdateTypeFlags.MessageReactionCount,
            UpdateType.ChatBoost => UpdateTypeFlags.ChatBoost,
            UpdateType.RemovedChatBoost => UpdateTypeFlags.RemovedChatBoost,
            UpdateType.BusinessConnection => UpdateTypeFlags.BusinessConnection,
            UpdateType.BusinessMessage => UpdateTypeFlags.BusinessMessage,
            UpdateType.EditedBusinessMessage => UpdateTypeFlags.EditedBusinessMessage,
            UpdateType.DeletedBusinessMessages => UpdateTypeFlags.DeletedBusinessMessages,
            UpdateType.PurchasedPaidMedia => UpdateTypeFlags.PurchasedPaidMedia,
            _ => UpdateTypeFlags.Unknown,
        };
}