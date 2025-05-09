using Telegram.Bot.Types.Payments;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.ShippingQuery"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of singleton update handler
/// <see cref="ShippingQueryHandler"/>.
/// </remarks>
/// <param name="callback">
/// A callback function that will be called when an <see cref="Update"/>
/// passes the <paramref name="filter"/>.
/// </param>
/// <param name="filter">
/// A filter to choose the right update to be handled inside
/// <paramref name="callback"/>.
/// </param>
/// <param name="endpoint">Determines if this is and endpoint handler.</param>
public sealed class ShippingQueryHandler(
    Func<IContainer<ShippingQuery>, Task> callback,
    IFilter<UpdaterFilterInputs<ShippingQuery>>? filter,
    bool endpoint = true)
    : DefaultHandler<ShippingQuery>(UpdateType.ShippingQuery, callback, filter, x => x.ShippingQuery, endpoint)
{
}
