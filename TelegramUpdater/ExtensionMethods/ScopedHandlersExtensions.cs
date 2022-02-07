using Microsoft.Extensions.Logging;
using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateHandlers.ScopedHandlers;

namespace TelegramUpdater
{
    public static class ScopedHandlersExtensions
    {
        /// <summary>
        /// Adds an scoped handler to the updater. ( Use this if you'r not sure. )
        /// </summary>
        /// <typeparam name="THandler">Handler type.</typeparam>
        /// <typeparam name="TUpdate">Update type.</typeparam>
        /// <param name="updateType">Update type again.</param>
        /// <param name="filter">A filter to choose the right update.</param>
        /// <param name="getT">
        /// A function to choose real update from <see cref="Update"/>
        /// <para>Don't touch it if you don't know.</para>
        /// </param>
        public static Updater AddScopedHandler<THandler, TUpdate>(
            this Updater updater,
            Filter<TUpdate>? filter = default,
            UpdateType? updateType = default,
            Func<Update, TUpdate>? getT = default)
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

            if (filter == null)
            {
                // If no filter passed as method args the look at attributes
                // Attribute filters are all combined using & operator.

                var applied = _h.GetCustomAttributes(typeof(ApplyFilterAttribute), false);
                foreach (ApplyFilterAttribute item in applied)
                {
                    var f = (Filter<TUpdate>?)Activator.CreateInstance(item.FilterType);
                    if (f != null)
                    {
                        if (filter == null)
                        {
                            filter = f;
                        }
                        else
                        {
                            filter &= f;
                        }
                    }
                }
            }

            return updater.AddScopedHandler(new UpdateContainerBuilder<THandler, TUpdate>(
                    updateType.Value, filter, getT));
        }

        /// <summary>
        /// Adds an scoped handler to the updater.
        /// </summary>
        /// <typeparam name="TUpdate">Update type.</typeparam>
        /// <param name="typeOfScopedHandler">Type of your handler.</param>
        /// <param name="updateType">Update type again.</param>
        /// <param name="filter">A filter to choose the right update.</param>
        /// <param name="getT">
        /// A function to choose real update from <see cref="Update"/>
        /// <para>Don't touch it if you don't know.</para>
        /// </param>
        public static Updater AddScopedHandler<TUpdate>(
            this Updater updater,
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

            if (filter == null)
            {
                // If no filter passed as method args the look at attributes
                // Attribute filters are all combined using & operator.

                var applied = typeOfScopedHandler.GetCustomAttributes(typeof(ApplyFilterAttribute), false);
                foreach (ApplyFilterAttribute item in applied)
                {
                    var f = (Filter<TUpdate>?)Activator.CreateInstance(item.FilterType);
                    if (f != null)
                    {
                        if (filter == null)
                        {
                            filter = f;
                        }
                        else
                        {
                            filter &= f;
                        }
                    }
                }
            }

            var containerGeneric = typeof(UpdateContainerBuilder<,>).MakeGenericType(
                typeOfScopedHandler, _t);

            var container = (IScopedHandlerContainer?)Activator.CreateInstance(
                containerGeneric, new object?[] { updateType.Value, filter, getT });

            if (container != null)
            {
                return updater.AddScopedHandler(container);
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
        /// Adds an scoped <see cref="Message"/> handler to the <paramref name="updater"/>.
        /// </summary>
        /// <typeparam name="THandler">Your <see cref="Message"/> handler type.</typeparam>
        /// <param name="updater"><see cref="Updater"/> itself.</param>
        /// <param name="filter">
        /// The filter to choose the right updates to handle.
        /// <para>
        /// Leave empty if you applied your fillter using <see cref="ApplyFilterAttribute"/> before.
        /// </para>
        /// </param>
        public static Updater AddScopedMessage<THandler>(this Updater updater,
                                                      Filter<Message>? filter = default)
            where THandler : IScopedUpdateHandler
            => updater.AddScopedHandler<THandler, Message>(
                filter, UpdateType.Message, x => x.Message!);

        /// <summary>
        /// Adds an scoped <see cref="CallbackQuery"/> handler to the <paramref name="updater"/>.
        /// </summary>
        /// <typeparam name="THandler">Your <see cref="CallbackQuery"/> handler type.</typeparam>
        /// <param name="updater"><see cref="Updater"/> itself.</param>
        /// <param name="filter">
        /// The filter to choose the right updates to handle.
        /// <para>
        /// Leave empty if you applied your fillter using <see cref="ApplyFilterAttribute"/> before.
        /// </para>
        /// </param>
        public static Updater AddScopedCallbackQuery<THandler>(this Updater updater,
                                                            Filter<CallbackQuery>? filter = default)
            where THandler : IScopedUpdateHandler
            => updater.AddScopedHandler<THandler, CallbackQuery>(
                filter, UpdateType.Message, x => x.CallbackQuery!);
    }
}
