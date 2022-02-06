using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers
{
    internal interface IScopedHandlerContainer
    {
        public Type ScopedHandlerType { get; }

        public UpdateType UpdateType { get; }

        public bool ShouldHandle(Update update);

        public IScopedUpdateHandler? CreateInstance()
        {
            return (IScopedUpdateHandler?)Activator.CreateInstance(ScopedHandlerType);
        }
    }
}
