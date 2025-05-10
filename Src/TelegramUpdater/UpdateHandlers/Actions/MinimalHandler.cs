using Microsoft.Extensions.DependencyInjection;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Singleton;

namespace TelegramUpdater.UpdateHandlers.Actions;

internal abstract class AbstractMinimalHandler<T, TContainer, Inputs>(
    UpdateType updateType,
    Func<TContainer, Inputs, CancellationToken, Task> callback,
    IFilter<UpdaterFilterInputs<T>>? filters = default,
    Func<Update, T?>? innerUpdateResolver = default,
    bool endpoint = true) : AbstractSingletonUpdateHandler<T, TContainer>(
        updateType, innerUpdateResolver, filters, endpoint)
    where TContainer : IContainer<T> where T : class where Inputs : notnull
{
    public Task Handle(
        TContainer container,
        Inputs inputs,
        CancellationToken cancellationToken = default) => callback(container, inputs, cancellationToken);

    protected virtual Inputs ResolveInputs(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<Inputs>();
    }

    public Task HandleAsync(
        HandlerInput input,
        IServiceScope? scope = null,
        CancellationToken cancellationToken = default)
    {
        if (scope?.ServiceProvider is null)
            throw new ArgumentNullException(nameof(scope), "ActionHandlers require a service provider.");

        return Handle(ContainerBuilder(input), ResolveInputs(scope.ServiceProvider), cancellationToken);
    }
}

internal class MinimalHandler<T, In1>(
    UpdateType updateType,
    Func<IContainer<T>, In1, CancellationToken, Task> callback,
    IFilter<UpdaterFilterInputs<T>>? filters = default,
    Func<Update, T?>? innerUpdateResolver = default,
    bool endpoint = true) : AbstractMinimalHandler<T, DefaultContainer<T>, In1>(
        updateType, callback, filters, innerUpdateResolver, endpoint)
    where T : class where In1 : notnull
{
    internal override DefaultContainer<T> ContainerBuilder(HandlerInput input)
        => new(InnerUpdateExtractor, input, Filter?.ExtraData);
}

internal class MinimalHandler<T, In1, In2>(
    UpdateType updateType,
    Func<IContainer<T>, In1, In2, CancellationToken, Task> callback,
    IFilter<UpdaterFilterInputs<T>>? filters = default,
    Func<Update, T?>? innerUpdateResolver = default,
    bool endpoint = true) : MinimalHandler<T, Tuple<In1, In2>>(
        updateType,
        (c, i, ct) => callback(c, i.Item1, i.Item2, ct),
        filters,
        innerUpdateResolver,
        endpoint)
    where T : class where In1 : notnull where In2 : notnull
{
    protected override Tuple<In1, In2> ResolveInputs(IServiceProvider serviceProvider)
    {
        var in1 = serviceProvider.GetRequiredService<In1>();
        var in2 = serviceProvider.GetRequiredService<In2>();
        return new Tuple<In1, In2>(in1, in2);
    }
}

/// <summary>
/// A set of static extension methods for <see cref="AbstractMinimalHandler{T, TContainer, Inputs}"/>s.
/// </summary>
public static class MinimalHandlersExtensions
{
    /// <summary>
    /// Adds an <see cref="MinimalHandler{T, In1}"/> to the updater.
    /// </summary>
    /// <remarks>
    /// This type of handlers are actually <see cref="ISingletonUpdateHandler"/>s, but they use
    /// <see cref="IServiceProvider"/> (like scoped handlers), to resolve extra arguments you pass in
    /// beside of first argument that is <see cref="IContainer{T}"/>.
    /// <para>
    /// Make sure you have access to the <see cref="IServiceProvider"/> or this will fail.
    /// </para>
    /// <para>
    /// Make sure <typeparamref name="In1"/> exists in the <see cref="IServiceCollection"/>.
    /// I will take care of the <see cref="IContainer{T}"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">Type of the update you want to handle.</typeparam>
    /// <typeparam name="In1">First argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
    public static IUpdater AddMinimalHandler<T, In1>(
        this IUpdater updater,
        UpdateType updateType,
        Func<IContainer<T>, In1, Task> callback,
        IFilter<UpdaterFilterInputs<T>>? filters = default,
        Func<Update, T?>? innerUpdateResolver = default,
        bool endpoint = true,
        HandlingOptions? options = default) where T : class where In1 : notnull
    {
        var action = new MinimalHandler<T, In1>(
            updateType,
            (c, in1, _) => callback(c, in1),
            filters,
            innerUpdateResolver,
            endpoint);

        return updater.AddSingletonUpdateHandler(action, options);
    }

    /// <summary>
    /// Adds an <see cref="MinimalHandler{T, In1}"/> to the updater.
    /// </summary>
    /// <remarks>
    /// This type of handlers are actually <see cref="ISingletonUpdateHandler"/>s, but they use
    /// <see cref="IServiceProvider"/> (like scoped handlers), to resolve extra arguments you pass in
    /// beside of first argument that is <see cref="IContainer{T}"/>.
    /// <para>
    /// Make sure you have access to the <see cref="IServiceProvider"/> or this will fail.
    /// </para>
    /// <para>
    /// Make sure <typeparamref name="In1"/> and <typeparamref name="In2"/> exists in the <see cref="IServiceCollection"/>.
    /// I will take care of the <see cref="IContainer{T}"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">Type of the update you want to handle.</typeparam>
    /// <typeparam name="In1">First argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
    /// <typeparam name="In2">Second argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
    public static IUpdater AddMinimalHandler<T, In1, In2>(
        this IUpdater updater,
        UpdateType updateType,
        Func<IContainer<T>, In1, In2, Task> callback,
        IFilter<UpdaterFilterInputs<T>>? filters = default,
        Func<Update, T?>? innerUpdateResolver = default,
        bool endpoint = true,
        HandlingOptions? options = default) where T : class where In1 : notnull where In2 : notnull
    {
        var action = new MinimalHandler<T, In1, In2>(
            updateType,
            (c, in1, in2, _) => callback(c, in1, in2),
            filters,
            innerUpdateResolver,
            endpoint);

        return updater.AddSingletonUpdateHandler(action, options);
    }
}
