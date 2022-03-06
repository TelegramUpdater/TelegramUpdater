using Telegram.Bot.Types.Payments;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.PreCheckoutQuery"/>.
/// </summary>
public sealed class PreCheckoutQueryHandler : AnyHandler<PreCheckoutQuery>
{
    /// <summary>
    /// Initialize a new instance of singleton update handler
    /// <see cref="PreCheckoutQueryHandler"/>.
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
    public PreCheckoutQueryHandler(Func<IContainer<PreCheckoutQuery>, Task> callback,
                                   IFilter<PreCheckoutQuery>? filter,
                                   int group)
        : base(UpdateType.PreCheckoutQuery, x=> x.PreCheckoutQuery, callback, filter, group)
    {
    }
}
