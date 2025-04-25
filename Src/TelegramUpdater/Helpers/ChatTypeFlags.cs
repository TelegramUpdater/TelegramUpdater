namespace TelegramUpdater.Helpers
{
    /// <summary>
    /// Flags shadow for <see cref="ChatType"/>.
    /// </summary>
    [Flags]
    public enum ChatTypeFlags
    {
        /// <summary>
        /// <see cref="ChatType.Private"/>.
        /// </summary>
        Private = 2,

        /// <summary>
        /// <see cref="ChatType.Group"/>.
        /// </summary>
        Group = 4,

        /// <summary>
        /// <see cref="ChatType.Channel"/>.
        /// </summary>
        Channel = 8,

        /// <summary>
        /// <see cref="ChatType.Supergroup"/>.
        /// </summary>
        SuperGroup = 16,

        /// <summary>
        /// <see cref="ChatType.Sender"/>.
        /// </summary>
        Sender = 32,
    }

    /// <summary>
    /// Extension methods for <see cref="ChatTypeFlags"/>.
    /// </summary>
    public static class ChatTypeExtensions
    {
        /// <summary>
        /// Converts a normal <see cref="ChatType"/> to a flag-able version.
        /// </summary>
        /// <param name="chatType">The chat type.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static ChatTypeFlags Flagable(this ChatType chatType)
        {
            return chatType switch
            {
                ChatType.Private => ChatTypeFlags.Private,
                ChatType.Group => ChatTypeFlags.Group,
                ChatType.Channel => ChatTypeFlags.Channel,
                ChatType.Supergroup => ChatTypeFlags.SuperGroup,
                ChatType.Sender => ChatTypeFlags.Sender,
                _ => throw new InvalidOperationException("Invalid chat type."),
            };
        }

        /// <summary>
        /// Checks if a flag-able version of <see cref="ChatType"/> ( <see cref="ChatTypeFlags"/> )
        /// is in <paramref name="flags"/>.
        /// </summary>
        /// <param name="chatType">The chat type.</param>
        /// <param name="flags">Flags to check.</param>
        /// <returns></returns>
        public static bool IsCorrect(this ChatType chatType, ChatTypeFlags flags)
        {
            return flags.HasFlag(chatType.Flagable());
        }
    }
}
