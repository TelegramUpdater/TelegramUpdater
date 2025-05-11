using Telegram.Bot.Types.Payments;

namespace TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

/// <summary>
/// Abstract scoped controller handler for <see cref="UpdateType.ShippingQuery"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class ShippingQueryControllerHandler()
    : DefaultControllerHandler<ShippingQuery>(x => x.ShippingQuery)
{
}
