using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.CallbackQuery"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of singleton update handler
/// <see cref="CallbackQueryHandler"/>.
/// </remarks>
/// <param name="callback">
/// A callback function that will be called when an <see cref="Update"/>
/// passes the <paramref name="filter"/>.
/// </param>
/// <param name="filter">
/// A filter to choose the right update to be handled inside
/// <paramref name="callback"/>.
/// </param>
/// <param name="endpoint"></param>
public class CallbackQueryHandler(
    Func<CallbackQueryContainer, Task> callback,
    IFilter<UpdaterFilterInputs<CallbackQuery>>? filter = default,
    bool endpoint = true)
    : AbstractSingletonUpdateHandler<CallbackQuery, CallbackQueryContainer>(
        updateType: UpdateType.CallbackQuery,
        getT: x => x.CallbackQuery,
        filter: filter,
        endpoint: endpoint)
{
    /// <inheritdoc/>
    protected override Task HandleAsync(CallbackQueryContainer container) => callback(container);

    internal override CallbackQueryContainer ContainerBuilder(HandlerInput input)
        => new(input, ExtraData);
}
