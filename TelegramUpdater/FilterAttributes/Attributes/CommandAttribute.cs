using System;
using Telegram.Bot.Types;
using TelegramUpdater.Filters;

namespace TelegramUpdater.FilterAttributes.Attributes
{
    /// <summary>
    /// An attribute for <see cref="CommandFilter"/>. Works only on <see cref="Message"/> handlers.
    /// </summary>
    public sealed class CommandAttribute : AbstractFilterAttribute
    {
        /// <summary>
        /// Filters messages with specified command
        /// </summary>
        /// <param name="commands">Command that are allowed. default prefix '/' will be applied!</param>
        public CommandAttribute(params string[] commands)
        {
            if (commands == null || commands.Length == 0)
                throw new ArgumentNullException(nameof(commands));

            Commands = commands;
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

            Commands = commands;
            Prefix = prefix;
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

            Prefix = prefix;
            ArgumentsMode = argumentsMode;
            Commands = commands;
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
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            Commands = new[] { command };
            Prefix = prefix;
            ArgumentsMode = argumentsMode;
        }

        internal string[] Commands { get; init; }

        internal char Prefix { get; init; } = '/';

        internal ArgumentsMode ArgumentsMode { get; init; } = ArgumentsMode.Idc;

        /// <inheritdoc/>
        protected internal override object GetFilterTypeOf(Type requestedType)
        {
            if (requestedType == null)
                throw new ArgumentNullException(nameof(requestedType));

            if (requestedType != typeof(Message))
                throw new ArgumentException("CommandAttribute are supported only for Messages.");

            return new CommandFilter(Prefix, ArgumentsMode, Commands);
        }
    }
}
