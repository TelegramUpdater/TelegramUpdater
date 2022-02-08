using System;
using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateHandlers.ScopedHandlers;

namespace TelegramUpdater.Asp
{
    public class UpdaterServiceBuilder
    {
        private readonly List<IScopedHandlerContainer> _scopedHandlerContainers;

        public UpdaterServiceBuilder()
        {
            _scopedHandlerContainers = new List<IScopedHandlerContainer>();
        }

        /// <summary>
        /// Add data to updater and discard this.
        /// </summary>
        public void AddToUpdater(Updater updater)
        {
            foreach (var container in _scopedHandlerContainers)
            {
                updater.AddScopedHandler(container);
            }

            _scopedHandlerContainers.Clear();
        }

        public IEnumerable<IScopedHandlerContainer> IterScopedContainers()
        {
            foreach (var container in _scopedHandlerContainers)
            {
                yield return container;
            }
        }

        /// <summary>
        /// Adds an scoped handler to the updater.
        /// </summary>
        /// <param name="scopedHandlerContainer">
        /// Use <see cref="UpdateContainerBuilder{THandler, TUpdate}"/>
        /// To Create a new <see cref="IScopedHandlerContainer"/>
        /// </param>
        public UpdaterServiceBuilder AddHandler(IScopedHandlerContainer scopedHandlerContainer)
        {
            var _h = scopedHandlerContainer.GetType();
            _scopedHandlerContainers.Add(scopedHandlerContainer);
            return this;
        }

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
        public UpdaterServiceBuilder AddHandler<THandler, TUpdate>(
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

            return AddHandler(new UpdateContainerBuilder<THandler, TUpdate>(
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
        public UpdaterServiceBuilder AddHandler<TUpdate>(
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
                return AddHandler(container);
            }
            else
            {
                throw new InvalidOperationException(
                    "Handler not added to the Scoped Handlers! The instance of it is null.");
            }
        }
    }
}
