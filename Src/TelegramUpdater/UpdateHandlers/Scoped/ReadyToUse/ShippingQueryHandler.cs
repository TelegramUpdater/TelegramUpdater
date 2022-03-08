using Telegram.Bot.Types.Payments;

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.ShippingQuery"/>.
/// </summary>
public abstract class ShippingQueryHandler : AnyHandler<ShippingQuery>
{
    /// <summary>
    /// Set handling priority of this handler.
    /// </summary>
    /// <param name="group">Handling priority group, The lower the sooner to process.</param>
    protected ShippingQueryHandler(int group = default)
        : base(x => x.ShippingQuery, group)
    {
    }
}
