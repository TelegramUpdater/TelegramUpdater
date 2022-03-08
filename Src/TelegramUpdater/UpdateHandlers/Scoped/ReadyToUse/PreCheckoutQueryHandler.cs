using Telegram.Bot.Types.Payments;

namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.PreCheckoutQuery"/>.
/// </summary>
public abstract class PreCheckoutQueryHandler : AnyHandler<PreCheckoutQuery>
{
    /// <summary>
    /// Set handling priority of this handler.
    /// </summary>
    /// <param name="group">Handling priority group, The lower the sooner to process.</param>
    protected PreCheckoutQueryHandler(int group = default)
        : base(x => x.PreCheckoutQuery, group)
    {
    }
}
