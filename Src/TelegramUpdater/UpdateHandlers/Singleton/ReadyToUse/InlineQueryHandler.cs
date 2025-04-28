// Ignore Spelling: Inline

using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.InlineQuery"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of singleton update handler
/// <see cref="InlineQueryHandler"/>.
/// </remarks>
/// <param name="callback">
/// A callback function that will be called when an <see cref="Update"/>
/// passes the <paramref name="filter"/>.
/// </param>
/// <param name="filter">
/// A filter to choose the right update to be handled inside
/// <paramref name="callback"/>.
/// </param>
public sealed class InlineQueryHandler(
    Func<IContainer<InlineQuery>, Task> callback,
    IFilter<UpdaterFilterInputs<InlineQuery>>? filter = default) 
    : AnyHandler<InlineQuery>(UpdateType.InlineQuery,
           x => x.InlineQuery,
           callback,
           filter)
{
}
