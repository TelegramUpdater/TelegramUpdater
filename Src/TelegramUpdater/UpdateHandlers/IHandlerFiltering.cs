namespace TelegramUpdater.UpdateHandlers;

/// <summary>
/// Handlers filtering. first using <see cref="UpdateType"/>
/// then using filters in <see cref="ShouldHandle(UpdaterFilterInputs{Update})"/>. 
/// </summary>
public interface IHandlerFiltering
{
    /// <summary>
    /// Your handler's update type. to see if it's matched.
    /// </summary>
    public UpdateType UpdateType { get; }

    /// <summary>
    /// Checks if an update can be handle in this handler.
    /// </summary>
    /// <returns></returns>
    public bool ShouldHandle(UpdaterFilterInputs<Update> inputs);
}

/// <summary>
/// Abstract base for <see cref="IHandlerFiltering"/>. 
/// </summary>
/// <typeparam name="T">Type of inner update.</typeparam>
public abstract class AbstractHandlerFiltering<T> : IHandlerFiltering where T : class
{
    /// <summary>
    /// Extract inner actual update.
    /// </summary>
    protected abstract Func<Update, T?> InnerUpdateExtractor { get; }

    /// <inheritdoc />
    protected abstract bool ShouldHandle(UpdaterFilterInputs<T> inputs);

    /// <inheritdoc />
    public abstract UpdateType UpdateType { get; }

    /// <inheritdoc />
    public bool ShouldHandle(UpdaterFilterInputs<Update> inputs)
    {
        if (inputs.Input.Type != UpdateType) return false;

        var insider = InnerUpdateExtractor(inputs.Input);

        if (insider == null) return false;

        return ShouldHandle(inputs.Rebase(insider));
    }
}
