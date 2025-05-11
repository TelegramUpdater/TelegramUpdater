using Microsoft.Extensions.DependencyInjection;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Singleton;

namespace TelegramUpdater.UpdateHandlers.Minimal;

/// <summary>
/// Base class for minimal handlers.
/// </summary>
/// <remarks>
/// Minimal handler are actually <see cref="ISingletonUpdateHandler"/>s that have access to the DI
/// so you should add them as singleton.
/// <para>
/// Use <see cref="MinimalHandler{T, In1}"/> and other overloads to instantiate one.
/// </para>
/// </remarks>
/// <typeparam name="T">Type of the input update.</typeparam>
/// <typeparam name="TContainer">Type of the container. which is mostly <see cref="DefaultContainer{T}"/></typeparam>
/// <typeparam name="Inputs">Your callback inputs other than <typeparamref name="TContainer"/>.</typeparam>
/// <param name="updateType">Type of the update.</param>
/// <param name="callback">Call back function.</param>
/// <param name="filters">Filters.</param>
/// <param name="innerUpdateResolver">Optionally pass a function that resolves <typeparamref name="T"/> from <see cref="Update"/></param>
/// <param name="endpoint">Determines if this an endpoint handler.</param>
public abstract class AbstractMinimalHandler<T, TContainer, Inputs>(
    UpdateType updateType,
    Func<TContainer, Inputs, CancellationToken, Task> callback,
    IFilter<UpdaterFilterInputs<T>>? filters = default,
    Func<Update, T?>? innerUpdateResolver = default,
    bool endpoint = true) : AbstractSingletonUpdateHandler<T, TContainer>(
        updateType, innerUpdateResolver, filters, endpoint)
    where TContainer : IContainer<T> where T : class where Inputs : notnull
{
    /// <summary>
    /// Resolve additional <typeparamref name="Inputs"/> from <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns></returns>
    protected virtual Inputs ResolveInputs(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<Inputs>();
    }

    /// <inheritdoc/>
    protected override Task HandleAsync(
        TContainer container,
        IServiceScope? scope = null,
        CancellationToken cancellationToken = default)
    {
        if (scope?.ServiceProvider is null)
            throw new ArgumentNullException(nameof(scope), "ActionHandlers require a service provider.");

        return callback(container, ResolveInputs(scope.ServiceProvider), cancellationToken);
    }
}
