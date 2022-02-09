using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Collections.Generic;
using TelegramUpdater.UpdateHandlers.ScopedHandlers;
using TelegramUpdater.ExceptionHandlers;
using System.Threading.Tasks;
using TelegramUpdater.UpdateHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace TelegramUpdater.Hosting
{
    public class UpdaterServiceBuilder
    {
        private readonly List<IScopedHandlerContainer> _scopedHandlerContainers;
        private readonly List<IExceptionHandler> _exceptionHandlers;

        public UpdaterServiceBuilder()
        {
            _scopedHandlerContainers = new List<IScopedHandlerContainer>();
            _exceptionHandlers = new List<IExceptionHandler>();
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

            foreach (var exceptionHandler in _exceptionHandlers)
            {
                updater.AddExceptionHandler(exceptionHandler);
            }

            _scopedHandlerContainers.Clear();
        }

        internal IEnumerable<IScopedHandlerContainer> IterScopedContainers()
        {
            foreach (var container in _scopedHandlerContainers)
            {
                yield return container;
            }
        }

        internal void AddToServiceCollection(IServiceCollection serviceDescriptors)
        {
            foreach (var container in IterScopedContainers())
            {
                serviceDescriptors.AddScoped(container.ScopedHandlerType);
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
            _scopedHandlerContainers.Add(scopedHandlerContainer);
            return this;
        }

        /// <summary>
        /// Add your exception handler to this updater.
        /// </summary>
        /// <param name="exceptionHandler"></param>
        public UpdaterServiceBuilder AddExceptionHandler(IExceptionHandler exceptionHandler)
        {
            _exceptionHandlers.Add(exceptionHandler);
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

        /// <summary>
        /// Adds an scoped <see cref="Message"/> handler to the <paramref name="updater"/>.
        /// </summary>
        /// <typeparam name="THandler">Your <see cref="Message"/> handler type.</typeparam>
        /// <param name="filter">
        /// The filter to choose the right updates to handle.
        /// <para>
        /// Leave empty if you applied your fillter using <see cref="ApplyFilterAttribute"/> before.
        /// </para>
        /// </param>
        public UpdaterServiceBuilder AddMessageHandler<THandler>(Filter<Message>? filter = default)
            where THandler : IScopedUpdateHandler
            => AddHandler<THandler, Message>(filter, UpdateType.Message, x => x.Message!);

        /// <summary>
        /// Adds an scoped <see cref="CallbackQuery"/> handler to the <paramref name="updater"/>.
        /// </summary>
        /// <typeparam name="THandler">Your <see cref="CallbackQuery"/> handler type.</typeparam>
        /// <param name="filter">
        /// The filter to choose the right updates to handle.
        /// <para>
        /// Leave empty if you applied your fillter using <see cref="ApplyFilterAttribute"/> before.
        /// </para>
        /// </param>
        public UpdaterServiceBuilder AddCallbackQueryHandler<THandler>(Filter<CallbackQuery>? filter = default)
            where THandler : IScopedUpdateHandler
            => AddHandler<THandler, CallbackQuery>(filter, UpdateType.Message, x => x.CallbackQuery!);

        /// <summary>
        /// Add your exception handler to this updater.
        /// </summary>
        /// <param name="callback">A callback function that will be called when the error catched.</param>
        /// <param name="messageMatch">Handle only when <see cref="Exception.Message"/> matches a text.</param>
        /// <param name="allowedHandlers">
        /// Handle only when the <see cref="Exception"/> occured in specified
        /// <see cref="IUpdateHandler"/>s
        /// <para>Leave null to handle all.</para>
        /// </param>
        public UpdaterServiceBuilder AddExceptionHandler<TException>(
            Func<Updater, Exception, Task> callback,
            Filter<string>? messageMatch = default,
            Type[]? allowedHandlers = null,
            bool inherit = false) where TException : Exception
        {
            return AddExceptionHandler(
                new ExceptionHandler<TException>(callback, messageMatch, allowedHandlers, inherit));
        }

        /// <summary>
        /// Add your exception handler to this updater.
        /// </summary>
        /// <param name="callback">A callback function that will be called when the error catched.</param>
        /// <param name="messageMatch">Handle only when <see cref="Exception.Message"/> matches a text.</param>
        public UpdaterServiceBuilder AddExceptionHandler<TException, THandler>(
            Func<Updater, Exception, Task> callback,
            Filter<string>? messageMatch = default,
            bool inherit = false)
            where TException : Exception where THandler : IUpdateHandler
        {
            return AddExceptionHandler<TException>(
                callback, messageMatch, new[] { typeof(THandler) }, inherit);
        }
    }
}
