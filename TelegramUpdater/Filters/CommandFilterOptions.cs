namespace TelegramUpdater.Filters;

/// <summary>
/// Select the mode of arguments fro command handler
/// </summary>
public enum ArgumentsMode
{
    /// <summary>
    /// If your command filter should have arguments
    /// </summary>
    Require,

    /// <summary>
    /// If your command filter should not have arguments
    /// </summary>
    NoArgs,

    /// <summary>
    /// If you don't care about your command filter arguments
    /// </summary>
    Idc
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
    public CommandFilterOptions(
        ArgumentsMode argumentsMode = ArgumentsMode.Idc,
        char separator = ' ',
        int? joinArgsFormIndex = default)
    {
        ArgumentsMode = argumentsMode;
        Separator = separator;
        JoinArgsFormIndex = joinArgsFormIndex;
    }

    /// <summary>
    /// Arguments mode.
    /// </summary>
    public ArgumentsMode ArgumentsMode { get; } = ArgumentsMode.Idc;

    /// <summary>
    /// Separator between arguments. default is ' '.
    /// </summary>
    public char Separator { get; } = ' ';

    /// <summary>
    /// If not null, the trailing augments starting this index are joined together
    /// using the <see cref="Separator"/>.
    /// </summary>
    public int? JoinArgsFormIndex { get; } = default;
}
