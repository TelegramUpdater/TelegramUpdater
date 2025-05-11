// Ignore Spelling: Inline

using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

/// <summary>
/// Sealed singleton update handler for <see cref="UpdateType.ChosenInlineResult"/>.
/// </summary>
/// <remarks>
/// Initialize a new instance of singleton update handler
/// <see cref="ChosenInlineResultHandler"/>.
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
public class ChosenInlineResultHandler(
    Func<IContainer<ChosenInlineResult>, Task> callback,
    IFilter<UpdaterFilterInputs<ChosenInlineResult>>? filter = default,
    bool endpoint = true) : DefaultHandler<ChosenInlineResult>(UpdateType.ChosenInlineResult,
          callback,
          filter,
          x => x.ChosenInlineResult,
          endpoint)
{
}
