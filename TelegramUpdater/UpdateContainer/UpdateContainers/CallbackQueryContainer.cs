using Telegram.Bot.Types;
using TelegramUpdater.RainbowUtlities;

namespace TelegramUpdater.UpdateContainer.UpdateContainers
{
    /// <summary>
    /// A container for <see cref="Update.CallbackQuery"/> only.
    /// </summary>
    public sealed class CallbackQueryContainer : UpdateContainerAbs<CallbackQuery>
    {
        internal CallbackQueryContainer(IUpdater updater, ShiningInfo<long, Update> shiningInfo)
            : base(x => x.CallbackQuery, updater, shiningInfo)
        {
        }
    }
}
