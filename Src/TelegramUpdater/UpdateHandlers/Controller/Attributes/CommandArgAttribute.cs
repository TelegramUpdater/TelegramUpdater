using TelegramUpdater.Filters;
using TelegramUpdater.Helpers;

namespace TelegramUpdater.UpdateHandlers.Controller.Attributes;

/// <summary>
/// This attribute is used to mark a parameter in a method as a <see cref="CommandFilter"/> argument.
/// </summary>
/// <param name="position">
/// The position of argument in command.
/// for command like <c>/start hello world</c>, the position of argument "hello" is 0 and "world" is 1.
/// <para>
/// The input argument should be parsed from string to the type of the parameter.
/// </para>
/// </param>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
public class CommandArgAttribute(int position) : ExtraDataAttribute("args")
{
    /// <summary>
    /// The position of argument in command.
    /// </summary>
    /// <remarks>
    /// for command like <c>/start hello world</c>, the position of argument "hello" is 0 and "world" is 1.
    /// <para>
    /// The input argument should be parsed from string to the type of the parameter.
    /// </para>
    /// </remarks>
    public int Position { get; } = position;

    /// <summary>
    /// Joins all arguments from the position to the end as one argument.
    /// </summary>
    public bool JoinToEnd { get; set; } = false;

    /// <summary>
    /// Separator for the arguments. If <see cref="JoinToEnd"/> is true.
    /// </summary>
    public char Separator { get; set; } = ' ';

    /// <inheritdoc/>
    protected internal override bool Polish(Type type, object? input, out object? output)
    {
        if (input is string[] args)
        {
            // Search for the argument in the array if exists
            if (args.Length > Position)
            {
                if (JoinToEnd)
                {
                    // Join all arguments from the position to the end as one argument
                    var joined = string.Join(Separator, args[Position..]);
                    if (joined.TryConvert(type, out output))
                    {
                        return true;
                    }
                }
                else
                {
                    // Try to convert the argument to the type of the parameter
                    if (args[Position].TryConvert(type, out output))
                    {
                        return true;
                    }
                }
            }
        }

        output = null;
        return false;
    }
}
