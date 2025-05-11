using Microsoft.Extensions.DependencyInjection;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Singleton;

namespace TelegramUpdater.UpdateHandlers.Minimal;

/// <summary>
/// Create a new minimal hander with one additional parameter
/// (<typeparamref name="In1"/>). which will be resolved from <see cref="IServiceProvider"/>.
/// </summary>
/// <remarks>
/// This is a <see cref="ISingletonUpdateHandler"/>.
/// </remarks>
/// <typeparam name="T">Type of input update.</typeparam>
/// <typeparam name="In1">First argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
/// <param name="updateType">Type of the update.</param>
/// <param name="callback">
/// Call back function.
/// <para>
/// Something like: <code>(<see cref="IContainer{T}"/> container, <typeparamref name="In1"/> input1) => (...)</code>
/// </para>
/// </param>
/// <param name="filters">Filters.</param>
/// <param name="innerUpdateResolver">Optionally pass a function that resolves <typeparamref name="T"/> from <see cref="Update"/></param>
/// <param name="endpoint">Determines if this an endpoint handler.</param>
public class MinimalHandler<T, In1>(
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

/// <summary>
/// Create a new minimal hander with two additional parameter
/// (<typeparamref name="In1"/>, <typeparamref name="In2"/>). which will be resolved from <see cref="IServiceProvider"/>.
/// </summary>
/// <remarks>
/// This is a <see cref="ISingletonUpdateHandler"/>.
/// </remarks>
/// <typeparam name="T">Type of input update.</typeparam>
/// <typeparam name="In1">First argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
/// <typeparam name="In2">Second argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>/// <param name="updateType">Type of the update.</param>
/// <param name="callback">
/// Call back function.
/// <para>
/// Something like: <code>(<see cref="IContainer{T}"/> container, <typeparamref name="In1"/> input1, <typeparamref name="In2"/> input2) => (...)</code>
/// </para>
/// </param>
/// <param name="filters">Filters.</param>
/// <param name="innerUpdateResolver">Optionally pass a function that resolves <typeparamref name="T"/> from <see cref="Update"/></param>
/// <param name="endpoint">Determines if this an endpoint handler.</param>
public class MinimalHandler<T, In1, In2>(
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
    /// <inheritdoc/>
    protected override Tuple<In1, In2> ResolveInputs(IServiceProvider serviceProvider)
    {
        var in1 = serviceProvider.GetRequiredService<In1>();
        var in2 = serviceProvider.GetRequiredService<In2>();
        return new Tuple<In1, In2>(in1, in2);
    }
}

/// <summary>
/// Create a new minimal hander with two additional parameter
/// (<typeparamref name="In1"/>, <typeparamref name="In2"/>, <typeparamref name="In3"/>). which will be resolved from <see cref="IServiceProvider"/>.
/// </summary>
/// <remarks>
/// This is a <see cref="ISingletonUpdateHandler"/>.
/// </remarks>
/// <typeparam name="T">Type of input update.</typeparam>
/// <typeparam name="In1">First argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
/// <typeparam name="In2">Second argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
/// <typeparam name="In3">Third argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>/// <param name="updateType">Type of the update.</param>
/// <param name="callback">
/// Call back function.
/// <para>
/// Something like: <code>(<see cref="IContainer{T}"/> container, <typeparamref name="In1"/> input1, <typeparamref name="In2"/>, <typeparamref name="In3"/> input3) => (...)</code>
/// </para>
/// </param>
/// <param name="filters">Filters.</param>
/// <param name="innerUpdateResolver">Optionally pass a function that resolves <typeparamref name="T"/> from <see cref="Update"/></param>
/// <param name="endpoint">Determines if this an endpoint handler.</param>
public class MinimalHandler<T, In1, In2, In3>(
    UpdateType updateType,
    Func<IContainer<T>, In1, In2, In3, CancellationToken, Task> callback,
    IFilter<UpdaterFilterInputs<T>>? filters = default,
    Func<Update, T?>? innerUpdateResolver = default,
    bool endpoint = true) : MinimalHandler<T, Tuple<In1, In2, In3>>(
        updateType,
        (c, i, ct) => callback(c, i.Item1, i.Item2, i.Item3, ct),
        filters,
        innerUpdateResolver,
        endpoint)
    where T : class where In1 : notnull where In2 : notnull where In3 : notnull
{
    /// <inheritdoc/>
    protected override Tuple<In1, In2, In3> ResolveInputs(IServiceProvider serviceProvider)
    {
        var in1 = serviceProvider.GetRequiredService<In1>();
        var in2 = serviceProvider.GetRequiredService<In2>();
        var in3 = serviceProvider.GetRequiredService<In3>();

        return new Tuple<In1, In2, In3>(in1, in2, in3);
    }
}

/// <summary>
/// Create a new minimal hander with two additional parameter
/// (<typeparamref name="In1"/>, <typeparamref name="In2"/>, <typeparamref name="In3"/>, <typeparamref name="In4"/>). which will be resolved from <see cref="IServiceProvider"/>.
/// </summary>
/// <remarks>
/// This is a <see cref="ISingletonUpdateHandler"/>.
/// </remarks>
/// <typeparam name="T">Type of input update.</typeparam>
/// <typeparam name="In1">First argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
/// <typeparam name="In2">Second argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
/// <typeparam name="In3">Third argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
/// <typeparam name="In4">Forth argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>/// <param name="updateType">Type of the update.</param>
/// <param name="callback">
/// Call back function.
/// <para>
/// Something like: <code>(<see cref="IContainer{T}"/> container, <typeparamref name="In1"/> input1, <typeparamref name="In2"/> input2, <typeparamref name="In3"/> input3, <typeparamref name="In4"/> input4) => (...)</code>
/// </para>
/// </param>
/// <param name="filters">Filters.</param>
/// <param name="innerUpdateResolver">Optionally pass a function that resolves <typeparamref name="T"/> from <see cref="Update"/></param>
/// <param name="endpoint">Determines if this an endpoint handler.</param>
public class MinimalHandler<T, In1, In2, In3, In4>(
    UpdateType updateType,
    Func<IContainer<T>, In1, In2, In3, In4, CancellationToken, Task> callback,
    IFilter<UpdaterFilterInputs<T>>? filters = default,
    Func<Update, T?>? innerUpdateResolver = default,
    bool endpoint = true) : MinimalHandler<T, Tuple<In1, In2, In3, In4>>(
        updateType,
        (c, i, ct) => callback(c, i.Item1, i.Item2, i.Item3, i.Item4, ct),
        filters,
        innerUpdateResolver,
        endpoint)
    where T : class where In1 : notnull where In2 : notnull where In3 : notnull where In4 : notnull
{
    /// <inheritdoc/>
    protected override Tuple<In1, In2, In3, In4> ResolveInputs(IServiceProvider serviceProvider)
    {
        var in1 = serviceProvider.GetRequiredService<In1>();
        var in2 = serviceProvider.GetRequiredService<In2>();
        var in3 = serviceProvider.GetRequiredService<In3>();
        var in4 = serviceProvider.GetRequiredService<In4>();

        return new Tuple<In1, In2, In3, In4>(in1, in2, in3, in4);
    }
}
