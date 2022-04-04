namespace TelegramUpdater.Filters;

/// <summary>
/// Select the mode of arguments fro command handler
/// </summary>
public enum ArgumentsMode
{
    /// <summary>
    /// If you don't care about your command filter arguments
    /// </summary>
    Idc = 0,

    /// <summary>
    /// If your command filter should have arguments
    /// </summary>
    Require,

    /// <summary>
    /// If your command filter should not have arguments
    /// </summary>
    NoArgs
}

/// <summary>
/// A set of options for command filter
/// </summary>
public readonly struct CommandFilterOptions
{
    /// <summary>
    /// Set command filter options.
    /// </summary>
    /// <param name="argumentsMode">Arguments mode.</param>
    /// <param name="separator">Separator between arguments. default is ' '.</param>
    /// <param name="joinArgsFormIndex">
    /// If not null, the trailing augments starting this index are joined together
    /// using the <see cref="Separator"/>.
    /// </param>
    /// <param name="descriptions">Provide description for every command at same other of commands.</param>
    /// <param name="botCommandScop">
    /// Command scope for this filter, This is only for setting commands and has no
    /// effects on updater or filters.
    /// </param>
    /// <param name="setCommandPriorities">
    /// Commands are ordered based on the when setting them.
    /// </param>
    /// <param name="botUsername">Username of the bot ( without @ ). will catch commands like /start@{username}.</param>
    /// <param name="caseSensitive">If command filter checks should be Case Sensitive.</param>
    public CommandFilterOptions(
        ArgumentsMode argumentsMode = ArgumentsMode.Idc,
        char separator = ' ',
        int? joinArgsFormIndex = default,
        string[]? descriptions = default,
        BotCommandScope? botCommandScop = default,
        int[]? setCommandPriorities = default,
        string? botUsername = default,
        bool caseSensitive = false)
    {
        ArgumentsMode = argumentsMode;
        Separator = separator;
        JoinArgsFormIndex = joinArgsFormIndex;
        Descriptions = descriptions;
        BotCommandScope = botCommandScop;
        SetCommandPriorities = setCommandPriorities;
        BotUsername = botUsername;
        CaseSensitive = caseSensitive;
    }

    /// <summary>
    /// Arguments mode.
    /// </summary>
    public ArgumentsMode ArgumentsMode { get; } = ArgumentsMode.Idc;

    /// <summary>
    /// If command filter checks should be Case Sensitive.
    /// </summary>
    public bool CaseSensitive { get; } = false;

    /// <summary>
    /// Separator between arguments. default is ' '.
    /// </summary>
    public char Separator { get; } = ' ';

    /// <summary>
    /// If not null, the trailing augments starting this index are joined together
    /// using the <see cref="Separator"/>.
    /// </summary>
    public int? JoinArgsFormIndex { get; } = default;

    /// <summary>
    /// Provide description for every command at same other of commands.
    /// </summary>
    /// <remarks>Description of the command, 3-256 characters.</remarks>
    public string[]? Descriptions { get; } = default;

    /// <summary>
    /// Command scope for this filter, This is only for setting commands and has no
    /// effects on updater or filters.
    /// </summary>
    public BotCommandScope? BotCommandScope { get; } = default;

    /// <summary>
    /// Commands are ordered based on the when setting them using
    /// <see cref="TelegramBotClientExtensions.SetMyCommandsAsync(
    /// ITelegramBotClient, IEnumerable{BotCommand}, BotCommandScope?,
    /// string?, CancellationToken)"/>.
    /// </summary>
    public int[]? SetCommandPriorities { get; } = default;

    /// <summary>
    /// Username of the bot ( without @ ). will catch commands like /start@{username}.
    /// </summary>
    /// <value></value>
    public string? BotUsername { get; } = default;
}
