using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.SealedHandlers
{
    public sealed class InlineQueryHandler : AnyUpdateHandler<InlineQuery>
    {
        public InlineQueryHandler(
            Func<IContainer<InlineQuery>, Task> callbak, Filter<InlineQuery>? filter, int group)
            : base(UpdateType.InlineQuery, x => x.InlineQuery, callbak, filter, group)
        {
        }
    }
}
