namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract <see cref="IScopedUpdateHandler"/> for
/// <see cref="Update.CallbackQuery"/>.
/// </summary>
public abstract class CallbackQueryHandler
    : AnyHandler<CallbackQuery>
{
    /// <summary>
    /// You can set handling priority in here.
    /// </summary>
    /// <param name="group">Handling priority.</param>
    protected CallbackQueryHandler(int group = 0)
        : base(x => x.CallbackQuery, group)
    {
    }
}
