using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse.Abstraction;

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.EditedChannelPost"/>.
/// </summary>
public abstract class EditedChannelPostHandler()
    : AbstractMessageHandler(x => x.EditedChannelPost)
{
}
