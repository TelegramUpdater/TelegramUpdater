using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.UpdateChannels
{
    /// <summary>
    /// An abstract class for channel updates.
    /// </summary>
    /// <typeparam name="T">Type of update to channel</typeparam>
    public abstract class AbstractChannel<T> : IUpdateChannel where T : class
    {
        private readonly Channel<IContainer<T>> _channel;
        private readonly CancellationTokenSource _tokenSource;
        private readonly Func<Update, T?> _getT;
        private readonly Filter<T>? _filter;
        private bool disposedValue;

        /// <summary>
        /// An abstract class for channel updates.
        /// </summary>
        /// <param name="updateType">Type of update.</param>
        /// <param name="getT">A function to select the right update from <see cref="Update"/></param>
        /// <param name="filter">Filter.</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected AbstractChannel(
            UpdateType updateType,
            Func<Update, T?> getT,
            Filter<T>? filter)
        {
            _filter = filter;
            UpdateType = updateType;
            _getT = getT ?? throw new ArgumentNullException(nameof(getT));
            _channel = Channel.CreateBounded<IContainer<T>>(new BoundedChannelOptions(1)
            {
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = true
            });
            _tokenSource = new CancellationTokenSource();
        }

        /// <inheritdoc/>
        public UpdateType UpdateType { get; }

        /// <summary>
        /// A function to select the right update from <see cref="Update"/>
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public T? GetT(Update update) => _getT(update);


        /// <inheritdoc/>
        public bool Cancelled => _tokenSource.IsCancellationRequested;

        /// <inheritdoc/>
        public async Task<IContainer<T>> ReadAsync(TimeSpan timeOut)
        {
            _tokenSource.CancelAfter(timeOut);
            return await _channel.Reader.ReadAsync(_tokenSource.Token);
        }

        /// <inheritdoc/>
        public async Task WriteAsync(IUpdater updater, Update update)
        {
            var container = ContainerBuilder(updater, update);
            await _channel.Writer.WriteAsync(container, _tokenSource.Token);
        }

        /// <inheritdoc/>
        protected bool ShouldChannel(T t)
        {
            if (_filter is null) return true;

            return _filter.TheyShellPass(t);
        }

        /// <summary>
        /// If this update should be channeled.
        /// </summary>
        public bool ShouldChannel(Update update)
        {
            var insider = GetT(update);

            if (insider == null) return false;

            return ShouldChannel(insider);
        }

        protected abstract IContainer<T> ContainerBuilder(IUpdater updater, Update update);

        /// <inheritdoc/>
        public void Cancel()
        {
            _tokenSource.Cancel();
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _tokenSource.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
