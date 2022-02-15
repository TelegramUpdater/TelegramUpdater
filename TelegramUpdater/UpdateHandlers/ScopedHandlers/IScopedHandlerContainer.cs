using Microsoft.Extensions.DependencyInjection;
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

        /// <summary>
        /// Checks if an update can be handled in a handler of type <see cref="ScopedHandlerType"/>.
        /// </summary>
        /// <param name="update">The update.</param>
        /// <returns></returns>
        public bool ShouldHandle(Update update);

        /// <summary>
        /// Initialize an instance of <see cref="ScopedHandlerType"/>.
        /// </summary>
        /// <param name="scope">If there is any <see cref="IServiceProvider"/> and <see cref="IServiceScope"/></param>
        /// <returns></returns>
        public IScopedUpdateHandler? CreateInstance(IServiceScope? scope = default)
        {
            if (scope != null)
            {
                return (IScopedUpdateHandler?)scope.ServiceProvider.GetRequiredService(
                    ScopedHandlerType);
            }
            else
            {
                return (IScopedUpdateHandler?)Activator.CreateInstance(ScopedHandlerType);
            }
        }
    }
}
