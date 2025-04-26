using TelegramUpdater.RainbowUtilities;

namespace TelegramUpdater.UpdateContainer.UpdateContainers;

/// <summary>
/// An update container for <see cref="Update.Message"/> only.
/// </summary>
public sealed class MessageContainer : AbstractUpdateContainer<Message>
{
    internal MessageContainer(
        IUpdater updater,
        ShiningInfo<long, Update> shiningInfo,
        IReadOnlyDictionary<string, object>? extraObjects = default)
        : base(x => x.Message, updater, shiningInfo, extraObjects)
    {
    }
}
