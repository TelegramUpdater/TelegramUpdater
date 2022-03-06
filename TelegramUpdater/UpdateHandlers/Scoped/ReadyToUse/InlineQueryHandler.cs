namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract <see cref="IScopedUpdateHandler"/> for
/// <see cref="Update.InlineQuery"/>.
/// </summary>
public abstract class InlineQueryHandler
    : AnyHandler<InlineQuery>
{
    /// <summary>
    /// You can set handling priority in here.
    /// </summary>
    /// <param name="group">Handling priority.</param>
    protected InlineQueryHandler(int group = default)
        : base(x => x.InlineQuery, group)
    {
    }


    #region Extension Methods
    /// <inheritdoc cref="InlineQuery.From"/>.
    protected User From => ActualUpdate.From;
    #endregion
}
