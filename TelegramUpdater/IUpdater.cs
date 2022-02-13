using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.UpdateChannels;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers;
using TelegramUpdater.UpdateHandlers.ScopedHandlers;

namespace TelegramUpdater
{
    /// <summary>
    /// Base interface for updater.
    /// </summary>
    public interface IUpdater
    {
        /// <summary>
        /// Bot client.
        /// </summary>
        ITelegramBotClient BotClient { get; }

        /// <summary>
        /// This updater logger.
        /// </summary>
        ILogger<IUpdater> Logger { get; }

        /// <summary>
        /// Options for this updater.
        /// </summary>
        UpdaterOptions UpdaterOptions { get; }

        /// <summary>
        /// Add your exception handler to this updater.
        /// </summary>
        /// <param name="exceptionHandler"></param>
        Updater AddExceptionHandler(IExceptionHandler exceptionHandler);

        /// <summary>
        /// Adds an scoped handler to the updater.
        /// </summary>
        /// <param name="scopedHandlerContainer">
        /// Use <see cref="UpdateContainerBuilder{THandler, TUpdate}"/>
        /// To Create a new <see cref="IScopedHandlerContainer"/>
        /// </param>
        Updater AddScopedHandler(IScopedHandlerContainer scopedHandlerContainer);

        /// <summary>
        /// Add your handler to this updater.
        /// </summary>
        /// <param name="updateHandler"></param>
        Updater AddUpdateHandler(ISingletonUpdateHandler updateHandler);

        /// <summary>
        /// Get current <see cref="TelegramBotClient"/>'s user information.
        /// </summary>
        /// <remarks>
        /// This method will cache! call freely.
        /// </remarks>
        Task<User> GetMeAsync();

        /// <summary>
        /// Opens a channel through the update handler and reads specified update.
        /// </summary>
        /// <typeparam name="T">Type of update you're excepting.</typeparam>
        /// <param name="updateChannel">An <see cref="IUpdateChannel"/></param>
        /// <param name="timeOut">Maximum allowed time to wait for that update.</param>
        Task<IContainer<T>?> OpenChannel<T>(AbstractChannel<T> updateChannel, TimeSpan timeOut) where T : class;

        /// <summary>
        /// Start handling updates.
        /// </summary>
        /// <param name="block">If this method should block the thread.</param>
        /// <param name="manualWriting">If you gonna write updates manually and no polling required.</param>
        /// <param name="cancellationToken">Use this to cancel the task.</param>
        /// <param name="fromServices">Indicates if there is service provider available.</param>
        Task Start(bool block = true, bool manualWriting = false, CancellationToken cancellationToken = default);
    }
}