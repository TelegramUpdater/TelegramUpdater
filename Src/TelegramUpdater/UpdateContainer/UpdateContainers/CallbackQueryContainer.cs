using TelegramUpdater.RainbowUtilities;

namespace TelegramUpdater.UpdateContainer.UpdateContainers;

/// <summary>
/// A container for <see cref="Update.CallbackQuery"/> only.
/// </summary>
public sealed class CallbackQueryContainer
    : AbstractUpdateContainer<CallbackQuery>
{
    internal CallbackQueryContainer(
        HandlerInput input,
        IReadOnlyDictionary<string, object>? extraObjects = default)
        : base(x => x.CallbackQuery, input, extraObjects)
    {
    }
}
