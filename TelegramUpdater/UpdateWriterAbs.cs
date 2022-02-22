using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramUpdater
{
    /// <summary>
    /// Use this class to create an update writer ( to the updater ).
    /// </summary>
    /// <remarks>
    /// If you're not using an <see cref="IServiceProvider"/>, the subclass you're building
    /// Should have parameterless constructor.
    /// <para>
    /// In case of <see cref="IServiceProvider"/>, This type and parameters should exists in service collection.
    /// </para>
    /// </remarks>
    public abstract class UpdateWriterAbs
    {
        /// <summary>
        /// Create a default instanse of update writer.
        /// </summary>
        protected UpdateWriterAbs() { }

        /// <summary>
        /// Create a default instanse of update writer.
        /// </summary>
        /// <param name="updater">The updater.</param>
        protected UpdateWriterAbs(IUpdater updater)
        {
            Updater = updater ?? throw new ArgumentNullException(nameof(updater));
        }

        internal void SetUpdater(IUpdater updater)
        {
            Updater = updater ?? throw new ArgumentNullException(nameof(updater));
        }

        /// <summary>
        /// Implement your stuff to get updates and write them to the updater
        /// Using <see cref="EnqueueUpdateAsync(Update, CancellationToken)"/>.
        /// </summary>
        /// <param name="stoppingToken">Cancel the job.</param>
        /// <returns></returns>
        public abstract Task ExecuteAsync(CancellationToken stoppingToken);

        /// <summary>
        /// Updater instance.
        /// </summary>
        protected IUpdater Updater { get; private set; } = null!;

        /// <summary>
        /// Telegram bot client instance.
        /// </summary>
        protected ITelegramBotClient BotClient => Updater.BotClient;

        /// <summary>
        /// Logger instance of this <see cref="Updater"/>
        /// </summary>
        protected ILogger<IUpdater> Logger => Updater.Logger;

        /// <summary>
        /// <see cref="Updater"/>'s options.
        /// </summary>
        protected UpdaterOptions UpdaterOptions => Updater.UpdaterOptions;

        /// <summary>
        /// Use this to add your update at the end of processing queue.
        /// </summary>
        /// <param name="update">The update.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async ValueTask EnqueueUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            await Updater.WriteAsync(update, cancellationToken);
        }

        internal static UpdateWriterAbs Create<TWriter>(IUpdater updater)
            where TWriter : UpdateWriterAbs
        {
            var writer = (UpdateWriterAbs?)Activator.CreateInstance(
                typeof(TWriter), new object[] { updater });

            if (writer == null)
                throw new InvalidOperationException($"Can't create {typeof(TWriter)}");
            return writer;
        }
    }
}
