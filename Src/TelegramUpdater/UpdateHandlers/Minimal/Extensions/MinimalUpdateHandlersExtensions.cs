using Microsoft.Extensions.DependencyInjection;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Singleton;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TelegramUpdater.UpdateHandlers.Minimal;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// A set of static extension methods for <see cref="AbstractMinimalUpdateHandler{T, TContainer, Inputs}"/>s.
/// </summary>
public static class MinimalUpdateHandlersExtensions
{
    /// <summary>
    /// Adds an <see cref="MinimalUpdateHandler{T, In1}"/> to the updater.
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
    public static IUpdater Handle<T, In1>(
        this IUpdater updater,
        UpdateType updateType,
        Func<IContainer<T>, In1, Task> callback,
        IFilter<UpdaterFilterInputs<T>>? filters = default,
        Func<Update, T?>? innerUpdateResolver = default,
        bool endpoint = true,
        HandlingOptions? options = default) where T : class where In1 : notnull
    {
        var action = new MinimalUpdateHandler<T, In1>(
            updateType,
            (c, in1, _) => callback(c, in1),
            filters,
            innerUpdateResolver,
            endpoint);

        return updater.AddSingletonUpdateHandler(action, options);
    }

    /// <summary>
    /// Adds an <see cref="MinimalUpdateHandler{T, In1, In2}"/> to the updater.
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
    public static IUpdater Handle<T, In1, In2>(
        this IUpdater updater,
        UpdateType updateType,
        Func<IContainer<T>, In1, In2, Task> callback,
        IFilter<UpdaterFilterInputs<T>>? filters = default,
        Func<Update, T?>? innerUpdateResolver = default,
        bool endpoint = true,
        HandlingOptions? options = default) where T : class where In1 : notnull where In2 : notnull
    {
        var action = new MinimalUpdateHandler<T, In1, In2>(
            updateType,
            (c, in1, in2, _) => callback(c, in1, in2),
            filters,
            innerUpdateResolver,
            endpoint);

        return updater.AddSingletonUpdateHandler(action, options);
    }

    /// <summary>
    /// Adds an <see cref="MinimalUpdateHandler{T, In1, In2, In3}"/> to the updater.
    /// </summary>
    /// <remarks>
    /// This type of handlers are actually <see cref="ISingletonUpdateHandler"/>s, but they use
    /// <see cref="IServiceProvider"/> (like scoped handlers), to resolve extra arguments you pass in
    /// beside of first argument that is <see cref="IContainer{T}"/>.
    /// <para>
    /// Make sure you have access to the <see cref="IServiceProvider"/> or this will fail.
    /// </para>
    /// <para>
    /// Make sure <typeparamref name="In1"/>, <typeparamref name="In2"/> and <typeparamref name="In3"/> exists in the <see cref="IServiceCollection"/>.
    /// I will take care of the <see cref="IContainer{T}"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">Type of the update you want to handle.</typeparam>
    /// <typeparam name="In1">First argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
    /// <typeparam name="In2">Second argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
    /// <typeparam name="In3">Third argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
    public static IUpdater Handle<T, In1, In2, In3>(
        this IUpdater updater,
        UpdateType updateType,
        Func<IContainer<T>, In1, In2, In3, Task> callback,
        IFilter<UpdaterFilterInputs<T>>? filters = default,
        Func<Update, T?>? innerUpdateResolver = default,
        bool endpoint = true,
        HandlingOptions? options = default) where T : class where In1 : notnull where In2 : notnull where In3 : notnull
    {
        var action = new MinimalUpdateHandler<T, In1, In2, In3>(
            updateType,
            (c, in1, in2, in3, _) => callback(c, in1, in2, in3),
            filters,
            innerUpdateResolver,
            endpoint);

        return updater.AddSingletonUpdateHandler(action, options);
    }

    /// <summary>
    /// Adds an <see cref="MinimalUpdateHandler{T, In1, In2, In3, In4}"/> to the updater.
    /// </summary>
    /// <remarks>
    /// This type of handlers are actually <see cref="ISingletonUpdateHandler"/>s, but they use
    /// <see cref="IServiceProvider"/> (like scoped handlers), to resolve extra arguments you pass in
    /// beside of first argument that is <see cref="IContainer{T}"/>.
    /// <para>
    /// Make sure you have access to the <see cref="IServiceProvider"/> or this will fail.
    /// </para>
    /// <para>
    /// Make sure <typeparamref name="In1"/>, <typeparamref name="In2"/>, <typeparamref name="In3"/>
    /// and <typeparamref name="In4"/> exists in the <see cref="IServiceCollection"/>.
    /// I will take care of the <see cref="IContainer{T}"/>.
    /// </para>
    /// <para>
    /// If you need to inject more than 4 services, why don't you use <see cref="ScopedUpdateHandlersExtensions.AddHandler{TUpdate}(IUpdater, Type, UpdateType, UpdaterFilter{TUpdate}?, Func{Update, TUpdate}?, HandlingOptions?)"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">Type of the update you want to handle.</typeparam>
    /// <typeparam name="In1">First argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
    /// <typeparam name="In2">Second argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
    /// <typeparam name="In3">Third argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
    /// <typeparam name="In4">Forth argument to be resolved by <see cref="IServiceProvider"/>.</typeparam>
    public static IUpdater Handle<T, In1, In2, In3, In4>(
        this IUpdater updater,
        UpdateType updateType,
        Func<IContainer<T>, In1, In2, In3, In4, Task> callback,
        IFilter<UpdaterFilterInputs<T>>? filters = default,
        Func<Update, T?>? innerUpdateResolver = default,
        bool endpoint = true,
        HandlingOptions? options = default) where T : class where In1 : notnull where In2 : notnull where In3 : notnull where In4 : notnull
    {
        var action = new MinimalUpdateHandler<T, In1, In2, In3, In4>(
            updateType,
            (c, in1, in2, in3, in4, _) => callback(c, in1, in2, in3, in4),
            filters,
            innerUpdateResolver,
            endpoint);

        return updater.AddSingletonUpdateHandler(action, options);
    }
}
