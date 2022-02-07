using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers
{
    /// <summary>
    /// Base interface for scoped update handlers container.
    /// </summary>
    public interface IScopedHandlerContainer
    {
        /// <summary>
        /// Type of <see cref="IScopedHandlerContainer"/>
        /// </summary>
        public Type ScopedHandlerType { get; }

        /// <summary>
        /// Your handler's update type.
        /// </summary>
        public UpdateType UpdateType { get; }

        public bool ShouldHandle(Update update);

        public IScopedUpdateHandler? CreateInstance()
        {
            return (IScopedUpdateHandler?)Activator.CreateInstance(ScopedHandlerType);
        }
    }
}
