using Telegram.Bot.Types.Payments;

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.ShippingQuery"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class ShippingQueryHandler()
    : DefaultHandler<ShippingQuery>(x => x.ShippingQuery)
{
}
