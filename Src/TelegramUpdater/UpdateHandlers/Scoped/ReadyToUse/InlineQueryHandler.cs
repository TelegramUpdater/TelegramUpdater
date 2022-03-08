namespace TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

/// <summary>
/// Abstract scoped update handler for <see cref="UpdateType.Message"/>.
/// </summary>
public abstract class InlineQueryHandler
    : AnyHandler<InlineQuery>
{
    /// <summary>
    /// Set handling priority of this handler.
    /// </summary>
    /// <param name="group">Handling priority group, The lower the sooner to process.</param>
    protected InlineQueryHandler(int group = default)
        : base(x => x.InlineQuery, group)
    {
    }


    #region Extension Methods
    /// <inheritdoc cref="InlineQuery.From"/>.
    protected User From => ActualUpdate.From;
    #endregion
}
