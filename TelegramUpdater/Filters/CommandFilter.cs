namespace TelegramUpdater.Filters
{
    /// <summary>
    /// Filters messages with specified command
    /// </summary>
    /// <remarks>
    /// <b>Extra data:</b> <see cref="IEnumerable{String}"/> "args".
    /// </remarks>
    public class CommandFilter : Filter<Message>
    {
        /// <summary>
        /// A set of commands to match without prefix.
        /// </summary>
        public string[] Commands { get; }

        /// <summary>
        /// Command prefix ( mainly '/' )
        /// </summary>
        public char Prefix { get; }

        /// <summary>
        /// Command filter options.
        /// </summary>
        public CommandFilterOptions Options { get; } = default;

        /// <summary>
        /// Filters messages with specified command
        /// </summary>
        /// <param name="commands">Command that are allowed. default prefix '/' will be applied!</param>
        public CommandFilter(params string[] commands) : base()
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
        public CommandFilter(char prefix, params string[] commands)
        {
            if (commands == null || commands.Length == 0)
                throw new ArgumentNullException(nameof(commands));

            Prefix = prefix;
            Commands = commands;
        }

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
        {
            if (commands == null || commands.Length == 0)
                throw new ArgumentNullException(nameof(commands));

            Commands = commands;
            Prefix = prefix;
            Options = new CommandFilterOptions(argumentsMode);
        }

        /// <summary>
        /// Filters messages with specified command
        /// </summary>
        /// <param name="command">Command that are allowed</param>
        /// <param name="prefix">Prefix of command. default to '/'</param>
        /// <param name="argumentsMode">If command should carry arguments</param>
        public CommandFilter(
            string command,
            char prefix = '/',
            ArgumentsMode argumentsMode = ArgumentsMode.Idc)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException($"'{nameof(command)}' cannot be null or empty.", nameof(command));
            }

            Commands = new[] { command };
            Prefix = prefix;
            Options = new CommandFilterOptions(argumentsMode);
        }

        /// <summary>
        /// Filters messages with specified command
        /// </summary>
        /// <param name="command">Command that are allowed</param>
        /// <param name="prefix">Prefix of command. default to '/'</param>
        /// <param name="options">Options for command filter.</param>
        public CommandFilter(
            string command,
            CommandFilterOptions options,
            char prefix = '/')
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException($"'{nameof(command)}' cannot be null or empty.", nameof(command));
            }

            Commands = new[] { command };
            Prefix = prefix;
            Options = options;
        }

        /// <inheritdoc/>
        public override bool TheyShellPass(Message input)
        {
            if (string.IsNullOrEmpty(input.Text)) return false;

            var args = input.Text.Split(Options.Separator);

            var command = args[0].ToLower();

            if (Options.ArgumentsMode == ArgumentsMode.Require &&
                args.Length < 2) return false;

            if (Options.ArgumentsMode == ArgumentsMode.NoArgs &&
                args.Length > 1) return false;

            var nakedArgs = args[1..];
            if (Options.JoinArgsFormIndex is not null)
            {
                var joined = string.Join(
                    Options.Separator,
                    nakedArgs[Options.JoinArgsFormIndex.Value..]);

                nakedArgs = nakedArgs[..Options.JoinArgsFormIndex.Value]
                    .Concat(new[] { joined }).ToArray();
            }

            AddOrUpdateData("args", nakedArgs);
            return Commands.Any(x => Prefix + x == command);
        }
    }
}
