// Ignore Spelling: Username

using TelegramUpdater.Filters;

namespace TelegramUpdater.FilterAttributes.Attributes;

/// <summary>
/// An attribute for <see cref="CommandFilter"/>. Works only on <see cref="Message"/> handlers.
/// </summary>
public sealed class CommandAttribute : AbstractUpdaterFilterAttribute
{
    /// <summary>
    /// Inner command filter.
    /// </summary>
    public CommandFilter Filter { get; }

    /// <summary>
    /// Filters messages with specified command
    /// </summary>
    /// <remarks>
    /// Text of the command, 1-32 characters. Can contain only lowercase English letters,
    /// digits and underscores.
    /// </remarks>
    /// <param name="commands">Command that are allowed. default prefix '/' will be applied!</param>
    public CommandAttribute(params string[] commands)
    {
        if (commands == null || commands.Length == 0)
            throw new ArgumentNullException(nameof(commands));

        Filter = new CommandFilter(commands);
    }

    /// <summary>
    /// Filters messages with specified command
    /// </summary>
    /// <remarks>
    /// Text of the command, 1-32 characters. Can contain only lowercase English letters,
    /// digits and underscores.
    /// </remarks>
    /// <param name="prefix">Prefix of command. default to '/'</param>
    /// <param name="commands">Command that are allowed</param>
    public CommandAttribute(char prefix, params string[] commands)
    {
        if (commands == null || commands.Length == 0)
            throw new ArgumentNullException(nameof(commands));

        Filter = new CommandFilter(prefix, commands);
    }

    /// <summary>
    /// Filters messages with specified command
    /// </summary>
    /// <remarks>
    /// Text of the command, 1-32 characters. Can contain only lowercase English letters,
    /// digits and underscores.
    /// </remarks>
    /// <param name="prefix">Prefix of command. default to '/'</param>
    /// <param name="commands">Command that are allowed</param>
    /// <param name="argumentsMode">If command should carry arguments</param>
    public CommandAttribute(
        char prefix,
        ArgumentsMode argumentsMode,
        params string[] commands)
    {
        if (commands == null || commands.Length == 0)
            throw new ArgumentNullException(nameof(commands));

        Filter = new CommandFilter(prefix, argumentsMode, commands);
    }

    /// <summary>
    /// Filters messages with specified command
    /// </summary>
    /// <remarks>
    /// Text of the command, 1-32 characters. Can contain only lowercase English letters,
    /// digits and underscores.
    /// </remarks>
    /// <param name="command">Command that are allowed</param>
    /// <param name="prefix">Prefix of command. default to '/'</param>
    /// <param name="argumentsMode">If command should carry arguments</param>
    public CommandAttribute(
        string command,
        char prefix = '/',
        ArgumentsMode argumentsMode = ArgumentsMode.Idc)
    {
        Filter = new CommandFilter(command, prefix, argumentsMode);
    }

    /// <summary>
    /// Filters messages with specified command
    /// </summary>
    /// <remarks>
    /// Text of the command, 1-32 characters. Can contain only lowercase English letters,
    /// digits and underscores.
    /// </remarks>
    /// <param name="command">Command that are allowed</param>
    /// <param name="prefix">Prefix of command. default to '/'</param>
    /// <param name="argumentsMode">Arguments mode.</param>
    /// <param name="separator">Separator between arguments. default is ' '.</param>
    /// <param name="joinArgsFormIndex">
    /// If not null, the trailing augments starting this index are joined together
    /// using the <paramref name="separator"/>.
    /// </param>
    public CommandAttribute(
        string command,
        int joinArgsFormIndex,
        char prefix = '/',
        ArgumentsMode argumentsMode = ArgumentsMode.Idc,
        char separator = ' ')
    {
        Filter = new CommandFilter(
            command, new CommandFilterOptions(
                argumentsMode, separator, joinArgsFormIndex),
            prefix);
    }

    /// <summary>
    /// Filters messages with specified command
    /// </summary>
    /// <remarks>
    /// Text of the command, 1-32 characters. Can contain only lowercase English letters,
    /// digits and underscores.
    /// </remarks>
    /// <param name="command">Command that are allowed</param>
    /// <param name="prefix">Prefix of command. default to '/'</param>
    /// <param name="argumentsMode">Arguments mode.</param>
    /// <param name="separator">Separator between arguments. default is ' '.</param>
    public CommandAttribute(
        string command,
        char separator,
        char prefix = '/',
        ArgumentsMode argumentsMode = ArgumentsMode.Idc)
    {
        Filter = new CommandFilter(
            command, new CommandFilterOptions(
                argumentsMode, separator, joinArgsFormIndex: null),
            prefix);
    }

    /// <summary>
    /// Filters messages with specified command
    /// </summary>
    /// <param name="commands">Allowed commands.</param>
    /// <param name="descriptions">Description for each <paramref name="commands"/>.</param>
    /// <param name="prefix">Command prefix.</param>
    /// <param name="argumentsMode">Command arguments mode.</param>
    /// <param name="separator">Separator char between arguments</param>
    /// <param name="joinArgs">If the trailing args should join from <paramref name="joinArgsFormIndex"/>.</param>
    /// <param name="joinArgsFormIndex">The index that args should join from.</param>
    /// <param name="botCommandScopeType">Scope type for the commands</param>
    /// <param name="setCommandPriorities">Commands are ordered based on the when setting them.</param>
    /// <param name="botUsername">Username of the bot ( without @ ). will catch commands like /start@{username}.</param>    
    /// <param name="caseSensitive">If command filter checks should be Case Sensitive.</param>
    public CommandAttribute(
        string[] commands,
        string[] descriptions,
        int[]? setCommandPriorities,
        char prefix = '/',
        ArgumentsMode argumentsMode = ArgumentsMode.Idc,
        char separator = ' ',
        bool joinArgs = false,
        int joinArgsFormIndex = 0,
        string botUsername = default!,
        BotCommandScopeType botCommandScopeType = BotCommandScopeType.Default,
        bool caseSensitive = default)
    {
        Filter = new CommandFilter(commands, prefix,
            new CommandFilterOptions(
                argumentsMode,
                separator,
                joinArgs ? joinArgsFormIndex : null,
                descriptions,
                ToBotCommandScope(botCommandScopeType),
                setCommandPriorities,
                botUsername,
                caseSensitive));
    }

    /// <summary>
    /// Filters messages with specified command
    /// </summary>
    /// <param name="command">Allowed command.</param>
    /// <param name="description">Description for <paramref name="command"/>.</param>
    /// <param name="prefix">Command prefix.</param>
    /// <param name="argumentsMode">Command arguments mode.</param>
    /// <param name="separator">Separator char between arguments</param>
    /// <param name="joinArgs">If the trailing args should join from <paramref name="joinArgsFormIndex"/>.</param>
    /// <param name="joinArgsFormIndex">The index that args should join from.</param>
    /// <param name="botCommandScopeType">Scope type for the commands</param>
    /// <param name="setCommandPriority">Commands are ordered based on the when setting them.</param>
    /// <param name="botUsername">Username of the bot ( without @ ). will catch commands like /start@{username}.</param>
    /// <param name="caseSensitive">If command filter checks should be Case Sensitive.</param>
    public CommandAttribute(
         string command,
         string description,
         int setCommandPriority = default,
         char prefix = '/',
         ArgumentsMode argumentsMode = ArgumentsMode.Idc,
         char separator = ' ',
         bool joinArgs = false,
         int joinArgsFormIndex = 0,
         string botUsername = default!,
         BotCommandScopeType botCommandScopeType = BotCommandScopeType.Default,
         bool caseSensitive = default)
    {
        Filter = new CommandFilter([command], prefix,
            new CommandFilterOptions(
                argumentsMode,
                separator,
                joinArgs ? joinArgsFormIndex : null,
                [description],
                ToBotCommandScope(botCommandScopeType),
                [setCommandPriority],
                botUsername,
                caseSensitive));
    }

    /// <summary>
    /// Create a DeepLinking filter attribute.
    /// </summary>
    /// <param name="joinArgs">Indicates if the trailing args should join.</param>
    /// <param name="deepLinkArg">
    /// Deep link argument:
    /// <para>https://t.me/botusername?start={<paramref name="deepLinkArg"/>}</para>
    /// <para><c>/start {<paramref name="deepLinkArg"/>}</c></para>
    /// </param>
    /// <param name="joinArgsFormIndex">
    /// If not null, the trailing augments starting this index are joined together
    /// using the <see cref="CommandFilterOptions.Separator"/>.
    /// </param>
    /// <param name="caseSensitive">If command filter checks should be Case Sensitive.</param>
    public CommandAttribute(
        string deepLinkArg,
        bool joinArgs = false,
        int joinArgsFormIndex = 0,
        bool caseSensitive = default)
    {
        Filter = new(deepLinkArg, joinArgs? joinArgsFormIndex: null, caseSensitive);
    }

    /// <inheritdoc/>
    protected internal override object GetUpdaterFilterTypeOf(Type requestedType)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(requestedType);
#else
        if (requestedType == null)
            throw new ArgumentNullException(nameof(requestedType));
#endif

        if (requestedType != typeof(Message))
            throw new ArgumentException("CommandAttribute are supported only for Messages.", nameof(requestedType));

        return Filter;
    }

    private static BotCommandScope ToBotCommandScope(BotCommandScopeType botCommandScopeType)
    {
        return botCommandScopeType switch
        {
            BotCommandScopeType.Default => new BotCommandScopeDefault(),
            BotCommandScopeType.AllPrivateChats => new BotCommandScopeAllPrivateChats(),
            BotCommandScopeType.AllGroupChats => new BotCommandScopeAllGroupChats(),
            BotCommandScopeType.AllChatAdministrators => new BotCommandScopeChatAdministrators(),
            BotCommandScopeType.Chat => new BotCommandScopeChat(),
            BotCommandScopeType.ChatAdministrators => new BotCommandScopeChatAdministrators(),
            BotCommandScopeType.ChatMember => new BotCommandScopeChatMember(),
            _ => throw new Exception("Invalid BotCommandScopeType"),
        };
    }
}
