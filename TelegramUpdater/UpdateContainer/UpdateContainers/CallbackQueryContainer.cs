using TelegramUpdater.RainbowUtlities;

namespace TelegramUpdater.UpdateContainer.UpdateContainers
{
    /// <summary>
    /// A container for <see cref="Update.CallbackQuery"/> only.
    /// </summary>
    public sealed class CallbackQueryContainer
        : UpdateContainerAbs<CallbackQuery>
    {
        internal CallbackQueryContainer(
            IUpdater updater,
            ShiningInfo<long, Update> shiningInfo,
            IReadOnlyDictionary<string, object>? extraObjects = default)
            : base(x => x.CallbackQuery, updater, shiningInfo, extraObjects)
        {
        }
    }
}
