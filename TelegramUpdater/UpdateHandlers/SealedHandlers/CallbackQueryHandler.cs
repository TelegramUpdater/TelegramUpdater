using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.SealedHandlers
{
    /// <summary>
    /// An update handler for <see cref="Update.CallbackQuery"/>.
    /// </summary>
    public sealed class CallbackQueryHandler : AnyUpdateHandler<CallbackQuery>
    {
        public CallbackQueryHandler(
            Func<IContainer<CallbackQuery>, Task> callbak, Filter<CallbackQuery>? filter, int group = 0)
            : base(UpdateType.CallbackQuery, x => x.CallbackQuery, callbak, filter, group)
        { }
    }
}
