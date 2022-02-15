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
        /// <remarks>
        /// This method enables auto writing updates from telegram to <see cref="Rainbow"/>.
        /// <para>
        /// If you wanna write updates yourself ( eg: webhook app ) use <see cref="StartReaderOnly(CancellationToken)"/>.
        /// </para>
        /// </remarks>
        /// <param name="cancellationToken">Use this to cancel the task.</param>
        void Start(CancellationToken cancellationToken = default);

        /// <summary>
        /// Start handling updates ( non blocking ).
        /// </summary>
        /// <remarks>
        /// This method dose not get any updates.
        /// and you should write them yourself using <see cref="WriteAsync(Update)"/>.
        /// <para>
        /// Use <see cref="Start(CancellationToken)"/> for an auto update writer.
        /// </para>
        /// </remarks>
        /// <param name="cancellationToken">Use this to cancel the task.</param>
        void StartReaderOnly(CancellationToken cancellationToken = default);

        /// <summary>
        /// Start handling updates ( blocking ).
        /// </summary>
        /// <remarks>
        /// This method enables auto writing updates from telegram to <see cref="Rainbow"/>.
        /// <para>
        /// If you wanna write updates yourself ( eg: webhook app ) use <see cref="StartReaderOnlyAsync(CancellationToken)"/>.
        /// </para>
        /// </remarks>
        /// <param name="cancellationToken">Use this to cancel the task.</param>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Start handling updates ( blocking ).
        /// </summary>
        /// <remarks>
        /// This method dose not get any updates.
        /// and you should write them yourself using <see cref="WriteAsync(Update)"/>.
        /// <para>
        /// Use <see cref="StartAsync(CancellationToken)"/> for an auto update writer.
        /// </para>
        /// </remarks>
        /// <param name="cancellationToken">Use this to cancel the task.</param>
        public Task StartReaderOnlyAsync(CancellationToken cancellationToken = default);
    }
}