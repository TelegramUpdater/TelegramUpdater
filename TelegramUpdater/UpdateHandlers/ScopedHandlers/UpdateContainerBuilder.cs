using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers
{
    /// <summary>
    /// Builds containers for <see cref="IScopedUpdateHandler"/>s.
    /// </summary>
    /// <typeparam name="THandler">The handler, which is <see cref="IScopedUpdateHandler"/></typeparam>
    /// <typeparam name="TUpdate">Update type.</typeparam>
    public sealed class UpdateContainerBuilder<THandler, TUpdate>
        : AbstractUpdateContainer<THandler, TUpdate>
        where THandler : IScopedUpdateHandler where TUpdate : class
    {
        private readonly Func<Update, TUpdate>? _getT;

        public UpdateContainerBuilder(
            UpdateType updateType,
            Filter<TUpdate>? filter = default,
            Func<Update, TUpdate>? getT = default)
            : base(updateType, filter)
        {
            _getT = getT;
        }

        protected override TUpdate? GetT(Update update)
        {
            if (_getT == null)
            {
                return update.GetInnerUpdate<TUpdate>();
            }
            else
            {
                return _getT(update);
            }
        }
    }
}
