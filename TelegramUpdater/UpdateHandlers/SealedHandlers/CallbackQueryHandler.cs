using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.SealedHandlers
{
    public sealed class CallbackQueryHandler : AnyUpdateHandler<CallbackQuery>
    {
        public CallbackQueryHandler(
            Func<UpdateContainerAbs<CallbackQuery>, Task> callbak, Filter<CallbackQuery>? filter, int group = 0)
            : base(UpdateType.CallbackQuery, x => x.CallbackQuery, callbak, filter, group)
        {
        }
    }
}
