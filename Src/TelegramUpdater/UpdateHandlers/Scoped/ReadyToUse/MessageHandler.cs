// Ignore Spelling: Webpage

using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse.Abstraction;

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.Message"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class MessageHandler() : AbstractMessageHandler(x => x.Message)
{
}
