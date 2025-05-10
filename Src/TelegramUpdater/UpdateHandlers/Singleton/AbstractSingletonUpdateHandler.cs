using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateHandlers.Singleton;

/// <summary>
/// Abstract base to create update handlers.
/// </summary>
/// <typeparam name="T">Update type.</typeparam>
/// <typeparam name="TContainer">Type of the container.</typeparam>
public abstract class AbstractSingletonUpdateHandler<T, TContainer>
    : AbstractHandlerFiltering<T>, IGenericSingletonUpdateHandler<T>
    where T : class
    where TContainer : IContainer<T>
{
    /// <summary>
    /// Create a new instance of <see cref="AbstractSingletonUpdateHandler{T, TContainer}"/>
    /// </summary>
    /// <param name="updateType">Target update type.</param>
    /// <param name="getT">To extract this update from <see cref="Update"/></param>
    /// <param name="filter">Filters.</param>
    /// <param name="endpoint">Determines if this is and endpoint handler.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    protected AbstractSingletonUpdateHandler(
        UpdateType updateType,
        Func<Update, T?>? getT = default,
        IFilter<UpdaterFilterInputs<T>>? filter = default,
        bool endpoint = true)
    {
        if (updateType == UpdateType.Unknown)
            throw new ArgumentException(
                $"There's nothing unknown here! {nameof(updateType)}", nameof(updateType));

        Filter = filter;
        GetActualUpdate = getT;
        UpdateType = updateType;
#pragma warning disable MA0056 // Do not call overridable members in constructor
        Endpoint = endpoint;
#pragma warning restore MA0056 // Do not call overridable members in constructor
    }

    internal IReadOnlyDictionary<string, object>? ExtraData
        => Filter?.ExtraData;

    /// <inheritdoc/>
    public IFilter<UpdaterFilterInputs<T>>? Filter { get; }

    /// <inheritdoc/>
    public Func<Update, T?>? GetActualUpdate { get; }

    /// <inheritdoc/>
    protected override Func<Update, T?> InnerUpdateExtractor
        => GetActualUpdate ?? ((update) => update.GetInnerUpdate<T>());

    /// <inheritdoc />
    public override UpdateType UpdateType { get; }

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
        return Task.CompletedTask;
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

    /// <summary>
    /// You can override this method instead of using filters.
    /// To apply a custom filter.
    /// </summary>
    /// <returns></returns>
    protected override bool ShouldHandle(UpdaterFilterInputs<T> inputs)
    {
        if (Filter is null) return true;

        return Filter.TheyShellPass(inputs);
    }

    /// <inheritdoc/>
    async Task IUpdateHandler.HandleAsync(
        HandlerInput input,
        IServiceScope? scope,
        CancellationToken cancellationToken)
    {
        var container = ContainerBuilder(input);
        await HandleAsync(container).ConfigureAwait(false);
    }

    internal abstract TContainer ContainerBuilder(HandlerInput input);

    /// <inheritdoc/>
    public virtual bool Endpoint { get; protected set; }
}
