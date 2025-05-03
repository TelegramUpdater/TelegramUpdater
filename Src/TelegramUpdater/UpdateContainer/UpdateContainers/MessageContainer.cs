using TelegramUpdater.RainbowUtilities;

namespace TelegramUpdater.UpdateContainer.UpdateContainers;

/// <summary>
/// An update container for <see cref="Update.Message"/> only.
/// </summary>
public sealed class MessageContainer : AbstractUpdateContainer<Message>
{
    internal MessageContainer(
        HandlerInput input,
        IReadOnlyDictionary<string, object>? extraObjects = default)
        : base(update => update.GetInnerUpdate<Message>(), input, extraObjects)
    {
    }
}
