using Telegram.Bot.Types.Payments;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.ShippingQuery"/>.
/// </summary>
public sealed class ShippingQueryHandler : AnyHandler<ShippingQuery>
{
    /// <summary>
    /// Initialize a new instance of singleton update handler
    /// <see cref="ShippingQueryHandler"/>.
    /// </summary>
    /// <param name="callback">
    /// A callback function that will be called when an <see cref="Update"/>
    /// passes the <paramref name="filter"/>.
    /// </param>
    /// <param name="filter">
    /// A filter to choose the right update to be handled inside
    /// <paramref name="callback"/>.
    /// </param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public ShippingQueryHandler(Func<IContainer<ShippingQuery>, Task> callback,
                                IFilter<ShippingQuery>? filter,
                                int group)
        : base(UpdateType.ShippingQuery, x=> x.ShippingQuery, callback, filter, group)
    {
    }
}
