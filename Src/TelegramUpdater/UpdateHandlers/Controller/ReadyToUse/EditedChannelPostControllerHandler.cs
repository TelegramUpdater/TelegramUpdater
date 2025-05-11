using TelegramUpdater.UpdateHandlers.Controller.ReadyToUse.Abstraction;

namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.EditedChannelPost"/>.
/// </summary>
public abstract class EditedChannelPostControllerHandler()
    : AbstractMessageControllerHandler(x => x.EditedChannelPost)
{
}
