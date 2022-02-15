using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.RainbowUtlities;
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
        /// Rainbow instance. resposeable for parallel queuing.
        /// </summary>
        public Rainbow<long, Update> Rainbow { get; }

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
        /// Manually write an update to the <see cref="Rainbow"/>
        /// </summary>
        Task WriteAsync(Update update);

        /// <summary>
        /// Start handling updates ( non blocking ).
        /// </summary>
        /// <param name="manualWriting">If you gonna write updates manually and no polling required.</param>
        /// <param name="cancellationToken">Use this to cancel the task.</param>
        void Start(bool manualWriting = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Start handling updates ( blocking ).
        /// </summary>
        /// <param name="manualWriting">If you gonna write updates manually and no polling required.</param>
        /// <param name="cancellationToken">Use this to cancel the task.</param>
        Task StartAsync(bool manualWriting = false, CancellationToken cancellationToken = default);
    }
}