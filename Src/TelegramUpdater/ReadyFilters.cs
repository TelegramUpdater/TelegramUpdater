// Ignore Spelling: Cutify

using System.Text.RegularExpressions;
using TelegramUpdater.Filters;
using TelegramUpdater.Helpers;

namespace TelegramUpdater;

/// <summary>
/// A collection of filters.
/// </summary>
public static class ReadyFilters
{
    /// <summary>
    /// The handler will always be triggered on specified update type of <typeparamref name="T"/>
    /// </summary>
    public static UpdaterFilter<T> Always<T>() => new((_, __) => true);

    /// <summary>
    /// The handler will be triggered when <paramref name="func"/> returns true
    /// on specified update type of <typeparamref name="T"/>
    /// </summary>
    public static UpdaterFilter<T> When<T>(Func<IUpdater, T, bool> func) => new(func);

    /// <summary>
    /// The handler will be triggered when <paramref name="func"/> passes
    /// on specified update type of <typeparamref name="T"/>
    /// </summary>
    public static UpdaterFilter<T>? When<T>(UpdaterFilter<T>? func) => func;

    /// <summary>
    /// The handler will be triggered when a message is a command specified in <paramref name="commands"/>
    /// </summary>
    public static UpdaterFilter<Message> OnCommand(params string[] commands)
        => new CommandFilter(commands);

    /// <summary>
    /// The handler will be triggered when a message is a command specified in <paramref name="commands"/>
    /// </summary>
    public static UpdaterFilter<Message> OnCommand(char prefix = '/', params string[] commands)
        => new CommandFilter(prefix, commands);

    /// <summary>
    /// The handler will be triggered when a regex matches its text.
    /// </summary>
    public static UpdaterFilter<Message> TextMatches(
        string pattern, bool catchCaption = false, RegexOptions? regexOptions = default)
            => new MessageTextRegex(pattern, catchCaption, regexOptions);

    /// <summary>
    /// The handler will be triggered when a regex matches its data.
    /// </summary>
    public static UpdaterFilter<CallbackQuery> DataMatches(
        string pattern, RegexOptions? regexOptions = default)
        => new CallbackQueryRegex(pattern, regexOptions);

    /// <summary>
    /// The handler will be triggered when a message sent in private chat
    /// </summary>
    public static UpdaterFilter<Message> PM() => new PrivateMessageFilter();

    /// <summary>
    /// Filter messages from <see cref="ChatType.Group"/> and <see cref="ChatType.Supergroup"/>.
    /// </summary>
    /// <returns></returns>
    public static UpdaterFilter<Message> Group() => InChatType(ChatTypeFlags.Group | ChatTypeFlags.SuperGroup);

    /// <summary>
    /// A message comes only from specified user(s) is <paramref name="users"/>
    /// </summary>
    public static UpdaterFilter<Message> MsgOfUsers(params long[] users)
        => FromUsersFilter.Messages(users);

    /// <summary>
    /// A callback query comes only from specified user(s) is <paramref name="users"/>
    /// </summary>
    public static UpdaterFilter<CallbackQuery> CbqOfUsers(params long[] users)
        => FromUsersFilter.CallbackQueries(users);

    /// <summary>
    /// A replied message.
    /// </summary>
    public static UpdaterFilter<Message> Replied() => new MessageRepliedFilter();

    /// <summary>
    /// Creates a filter that checks if a selected property is not null.
    /// </summary>
    /// <param name="selector">Function to select a property out of <typeparamref name="T"/></param>
    /// <returns></returns>
    public static UpdaterFilter<K> NotNullFilter<K, T>(Func<K, T?> selector)
        where T : class => new((_, x) => selector(x) != null);

    /// <summary>
    /// Filters text messages.
    /// </summary>
    public static UpdaterFilter<Message> Text() => MessageTypeOf(MessageType.Text);

    /// <summary>
    /// Filters messages with caption.
    /// </summary>
    public static UpdaterFilter<Message> Caption()
        => NotNullFilter<Message, string>(x => x.Caption);

    /// <summary>
    /// Filters photo messages.
    /// </summary>
    public static UpdaterFilter<Message> Photo() => MessageTypeOf(MessageType.Photo);

    /// <summary>
    /// Filters video messages.
    /// </summary>
    public static UpdaterFilter<Message> Video() => MessageTypeOf(MessageType.Video);

    /// <summary>
    /// Filter <see cref="Message"/>s by <see cref="ChatTypeFlags"/> ( flags version of <see cref="ChatType"/> ).
    /// </summary>
    /// <param name="chatTypeFlags">Chat type flags.</param>
    /// <returns></returns>
    public static UpdaterFilter<Message> InChatType(ChatTypeFlags chatTypeFlags)
        => new((_, x) => x.Chat.Type.IsCorrect(chatTypeFlags));

    /// <summary>
    /// Filter messages of type <paramref name="messageType"/>.
    /// </summary>
    /// <param name="messageType">Type of the message - text, video ...</param>
    /// <returns></returns>
    public static UpdaterFilter<Message> MessageTypeOf(MessageType messageType)
        => new((_, x) => x.Type == messageType);

}
