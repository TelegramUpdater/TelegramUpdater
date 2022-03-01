using TelegramUpdater.Filters;

namespace TelegramUpdater.FilterAttributes.Attributes
{
    /// <summary>
    /// An attribute for <see cref="CommandFilter"/>. Works only on <see cref="Message"/> handlers.
    /// </summary>
    public sealed class CommandAttribute : AbstractFilterAttribute
    {
        /// <summary>
        /// Inner command filter.
        /// </summary>
        public CommandFilter Filter { get; }

        /// <summary>
        /// Filters messages with specified command
        /// </summary>
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
            char prefix = '/',
            ArgumentsMode argumentsMode = ArgumentsMode.Idc,
            char separator = ' ',
            int? joinArgsFormIndex = default)
        {
            Filter = new CommandFilter(
                command, new CommandFilterOptions(
                    argumentsMode, separator, joinArgsFormIndex),
                prefix);
        }

        /// <summary>
        /// Filters messages with specified command
        /// </summary>
        /// <param name="command">Command that are allowed</param>
        /// <param name="prefix">Prefix of command. default to '/'</param>
        /// <param name="argumentsMode">Arguments mode.</param>
        /// <param name="separator">Separator between arguments. default is ' '.</param>
        public CommandAttribute(
            string command,
            char prefix = '/',
            ArgumentsMode argumentsMode = ArgumentsMode.Idc,
            char separator = ' ')
        {
            Filter = new CommandFilter(
                command, new CommandFilterOptions(
                    argumentsMode, separator, null),
                prefix);
        }

        /// <inheritdoc/>
        protected internal override object GetFilterTypeOf(Type requestedType)
        {
            if (requestedType == null)
                throw new ArgumentNullException(nameof(requestedType));

            if (requestedType != typeof(Message))
                throw new ArgumentException("CommandAttribute are supported only for Messages.");

            return Filter;
        }
    }
}
