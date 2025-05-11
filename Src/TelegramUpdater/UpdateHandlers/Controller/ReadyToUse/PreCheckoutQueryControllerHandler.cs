using Telegram.Bot.Types.Payments;

namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.PreCheckoutQuery"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class PreCheckoutQueryControllerHandler()
    : DefaultControllerHandler<PreCheckoutQuery>(x => x.PreCheckoutQuery)
{
}
