using System.Linq;
using Telegram.Bot.Types;

namespace TelegramUpdater.Filters
{
    /// <summary>
    /// Filters messages with specified command
    /// </summary>
    public class CommandFilter : Filter<Message>
    {
        private static bool _CommandFilter(
            Message incoming,
            char prefix = '/',
            ArgumentsMode argumentsMode = ArgumentsMode.Idc,
            params string[] commands)
        {
            if (string.IsNullOrEmpty(incoming.Text)) return false;

            var args = incoming.Text.Split(' ');

            var command = args[0].ToLower();

            if (argumentsMode == ArgumentsMode.Require &&
                args.Length < 2) return false;

            if (argumentsMode == ArgumentsMode.NoArgs &&
                args.Length > 1) return false;

            return commands.Any(x => prefix + x == command);
        }

        /// <summary>
        /// Filters messages with specified command
        /// </summary>
        /// <param name="commands">Command that are allowed. default prefix '/' will be applied!</param>
        public CommandFilter(params string[] commands)
            : base(x =>
            {
                return _CommandFilter(x, '/', ArgumentsMode.Idc, commands);
            })
        { }

        /// <summary>
        /// Filters messages with specified command
        /// </summary>
        /// <param name="prefix">Prefix of command. default to '/'</param>
        /// <param name="commands">Command that are allowed</param>
        public CommandFilter(char prefix, params string[] commands)
            : base(x =>
            {
                return _CommandFilter(x, prefix, ArgumentsMode.Idc, commands);
            })
        { }

        /// <summary>
        /// Filters messages with specified command
        /// </summary>
        /// <param name="prefix">Prefix of command. default to '/'</param>
        /// <param name="commands">Command that are allowed</param>
        /// <param name="argumentsMode">If command should carry arguments</param>
        public CommandFilter(
            char prefix,
            ArgumentsMode argumentsMode,
            params string[] commands)
            : base(x =>
            {
                return _CommandFilter(x, prefix, argumentsMode, commands);
            })
        { }

        /// <summary>
        /// Filters messages with specified command
        /// </summary>
        /// <param name="commands">Command that are allowed</param>
        /// <param name="prefix">Prefix of command. default to '/'</param>
        /// <param name="argumentsMode">If command should carry arguments</param>
        public CommandFilter(
            string commands,
            char prefix = '/',
            ArgumentsMode argumentsMode = ArgumentsMode.Idc)
            : base(x =>
            {
                return _CommandFilter(x, prefix, argumentsMode, commands);
            })
        { }
    }
}
