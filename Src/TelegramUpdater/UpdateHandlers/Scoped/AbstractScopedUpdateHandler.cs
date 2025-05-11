using Microsoft.Extensions.DependencyInjection;
using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Scoped;

/// <summary>
/// Abstract base for <see cref="IScopedUpdateHandler"/>s.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Create a new instance of <see cref="AbstractScopedUpdateHandler{T, TContainer}"/>.
/// </remarks>
/// <param name="getT">Extract actual update from <see cref="Update"/>.</param>
/// <typeparam name="TContainer">Type of the container.</typeparam>
/// <exception cref="ArgumentNullException"></exception>
public abstract class AbstractScopedUpdateHandler<T, TContainer>(Func<Update, T?>? getT)
    : AbstractHandlerProvider<T>, IScopedUpdateHandler
    where T : class
    where TContainer : IContainer<T>
{
    private readonly Func<Update, T?> _getT = getT ?? ((updater) => updater.GetInnerUpdate<T>());
    private IReadOnlyDictionary<string, object>? _extraData;

    IReadOnlyDictionary<string, object>? IScopedUpdateHandler.ExtraData
        => _extraData;

    internal IReadOnlyDictionary<string, object>? ExtraData
        => _extraData;

    /// <summary>
    /// Here you may handle the incoming update.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Override <b>ONLY ONE</b> of HandleAsync methods.
    /// </para>
    /// </remarks>
    /// <returns></returns>
    protected virtual Task HandleAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Here you may handle the incoming update.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Override <b>ONLY ONE</b> of HandleAsync methods.
    /// </para>
    /// </remarks>
    /// <param name="container">
    /// Provides everything you need and everything you want!
    /// </param>
    /// <returns></returns>
    protected virtual Task HandleAsync(TContainer container)
    {
        return HandleAsync();
    }

    /// <summary>
    /// Here you may handle the incoming update.
    /// </summary>
    /// <remarks>
    /// This method has all initial arguments.
    /// <para>
    /// Override <b>ONLY ONE</b> of HandleAsync methods.
    /// </para>
    /// </remarks>
    /// <param name="container">
    /// Provides everything you need and everything you want!
    /// </param>
    /// <param name="scope">
    /// The service scope associated with this handler if any exists.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task HandleAsync(
        TContainer container,
        IServiceScope? scope = default,
        CancellationToken cancellationToken = default)
    {
        return HandleAsync(container);
    }

    /// <inheritdoc/>
    Task IUpdateHandler.HandleAsync(
        HandlerInput input,
        IServiceScope? scope,
        CancellationToken cancellationToken)
    {
        return HandleAsync(ContainerBuilderWrapper(input), scope, cancellationToken);
    }

    void IScopedUpdateHandler.SetExtraData(
        IReadOnlyDictionary<string, object>? extraData)
        => _extraData = extraData;

    /// <summary>
    /// A function to extract actual update from <see cref="Update"/>.
    /// </summary>
    /// <param name="update">The update.</param>
    /// <returns></returns>
    internal protected T? GetT(Update update) => _getT(update);

    /// <summary>
    /// Create update container for this handler.
    /// </summary>
    internal protected abstract TContainer ContainerBuilder(HandlerInput input);

    private TContainer ContainerBuilderWrapper(HandlerInput input)
    {
        var container = ContainerBuilder(input);
        Container = container;
        return container;
    }

    /// <inheritdoc/>
    public override IContainer<T> Container { get; protected set; } = default!;

    /// <inheritdoc/>
    public virtual bool Endpoint { get; protected set; } = true;
}
