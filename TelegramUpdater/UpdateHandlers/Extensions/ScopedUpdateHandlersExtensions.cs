﻿using Microsoft.Extensions.Logging;
using TelegramUpdater.UpdateHandlers.Scoped;

namespace TelegramUpdater;

/// <summary>
/// A set of useful extension methods for <see cref="IScopedUpdateHandler"/>s.
/// </summary>
public static class ScopedUpdateHandlersExtensions
{
    /// <summary>
    /// Adds an scoped handler to the updater.
    /// </summary>
    /// <typeparam name="TUpdate">Update type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="typeOfScopedHandler">Type of your handler.</param>
    /// <param name="updateType">Update type again.</param>
    /// <param name="filter">A filter to choose the right update.</param>
    /// <param name="getT">
    /// A function to choose real update from <see cref="Update"/>
    /// <para>Don't touch it if you don't know.</para>
    /// </param>
    /// <remarks>This method will add filter attributes if <paramref name="filter"/> is null.</remarks>
    public static IUpdater AddScopedUpdateHandler<TUpdate>(this IUpdater updater,
                                                     Type typeOfScopedHandler,
                                                     Filter<TUpdate>? filter = default,
                                                     UpdateType? updateType = default,
                                                     Func<Update, TUpdate>? getT = default) where TUpdate : class
    {
        if (!typeof(IScopedUpdateHandler).IsAssignableFrom(typeOfScopedHandler))
        {
            throw new InvalidCastException($"{typeOfScopedHandler} Should be an IScopedUpdateHandler");
        }

        var _t = typeof(TUpdate);
        if (updateType == null)
        {
            if (Enum.TryParse(_t.Name, out UpdateType ut))
            {
                updateType = ut;
            }
            else
            {
                throw new InvalidCastException($"{_t} is not an Update! Should be Message, CallbackQuery, ...");
            }
        }

        var containerGeneric = typeof(ScopedUpdateHandlerContainerBuilder<,>).MakeGenericType(
            typeOfScopedHandler, _t);

        var container = (IScopedUpdateHandlerContainer?)Activator.CreateInstance(
            containerGeneric, new object?[] { updateType.Value, filter, getT });

        if (container != null)
        {
            return updater.AddScopedUpdateHandler(container);
        }
        else
        {
            updater.Logger.LogWarning(
                "{type} not added to the Scoped Handlers! The instance of it is null.",
                typeOfScopedHandler);
            throw new InvalidOperationException(
                "Handler not added to the Scoped Handlers! The instance of it is null.");
        }
    }

    /// <summary>
    /// Adds an scoped handler to the updater. ( Use this if you'r not sure. )
    /// </summary>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <typeparam name="TUpdate">Update type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="updateType">Update type again.</param>
    /// <param name="filter">A filter to choose the right update.</param>
    /// <param name="getT">
    /// A function to choose real update from <see cref="Update"/>
    /// <para>Don't touch it if you don't know.</para>
    /// </param>
    /// <remarks>This method will add filter attributes if <paramref name="filter"/> is null.</remarks>
    public static IUpdater AddScopedUpdateHandler<THandler, TUpdate>(
        this IUpdater updater,
        Filter<TUpdate>? filter = default,
        UpdateType? updateType = default,
        Func<Update, TUpdate?>? getT = default)
        where THandler : IScopedUpdateHandler where TUpdate : class
    {
        if (updateType == null)
        {
            var _t = typeof(TUpdate);
            if (Enum.TryParse(_t.Name, out UpdateType ut))
            {
                updateType = ut;
            }
            else
            {
                throw new InvalidCastException($"{_t} is not an Update! Should be Message, CallbackQuery, ...");
            }
        }

        var _h = typeof(THandler);

        return updater.AddScopedUpdateHandler(new ScopedUpdateHandlerContainerBuilder<THandler, TUpdate>(
                updateType.Value, filter, getT));
    }

    /// <summary>
    /// Adds an scoped <see cref="Message"/> handler to the updater. ( Use this if you'r not sure. )
    /// </summary>
    /// <remarks>This method will add filter attributes if <paramref name="filter"/> is null.</remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">A filter to choose the right update.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        Filter<Message>? filter = default) where THandler : IScopedUpdateHandler
    {
        return updater.AddScopedUpdateHandler<THandler, Message>(
            filter, UpdateType.Message, x => x.Message);
    }

    /// <summary>
    /// Adds an scoped <see cref="CallbackQuery"/> handler to the updater. ( Use this if you'r not sure. )
    /// </summary>
    /// <remarks>This method will add filter attributes if <paramref name="filter"/> is null.</remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">A filter to choose the right update.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        Filter<CallbackQuery>? filter = default) where THandler : IScopedUpdateHandler
    {
        return updater.AddScopedUpdateHandler<THandler, CallbackQuery>(
            filter, UpdateType.CallbackQuery, x => x.CallbackQuery);
    }

    /// <summary>
    /// Adds an scoped <see cref="InlineQuery"/> handler to the updater. ( Use this if you'r not sure. )
    /// </summary>
    /// <remarks>This method will add filter attributes if <paramref name="filter"/> is null.</remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">A filter to choose the right update.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        Filter<InlineQuery>? filter = default) where THandler : IScopedUpdateHandler
    {
        return updater.AddScopedUpdateHandler<THandler, InlineQuery>(
            filter, UpdateType.InlineQuery, x => x.InlineQuery);
    }
}