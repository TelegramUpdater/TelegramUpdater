using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.InlineQuery"/>.
/// </summary>
public sealed class InlineQueryHandler : AnyHandler<InlineQuery>
{
    /// <summary>
    /// Initialize a new instance of singleton update handler
    /// <see cref="InlineQueryHandler"/>.
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
    public InlineQueryHandler(Func<IContainer<InlineQuery>, Task> callback,
                              IFilter<InlineQuery>? filter = default,
                              int group = default)
        : base(UpdateType.InlineQuery,
               x => x.InlineQuery,
               callback,
               filter,
               group)
    { }
}
