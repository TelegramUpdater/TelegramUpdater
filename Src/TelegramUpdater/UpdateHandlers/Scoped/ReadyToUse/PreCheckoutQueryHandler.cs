using Telegram.Bot.Types.Payments;

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.PreCheckoutQuery"/>.
/// </summary>
/// <remarks>
/// Set handling priority of this handler.
/// </remarks>
public abstract class PreCheckoutQueryHandler()
    : AnyHandler<PreCheckoutQuery>(x => x.PreCheckoutQuery)
{
}
