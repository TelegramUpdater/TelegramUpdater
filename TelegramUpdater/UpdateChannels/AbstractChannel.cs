using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.UpdateChannels
{
    public abstract class AbstractChannel<T>: IUpdateChannel where T : class
    {
        private readonly Channel<Update> _channel;
        private readonly CancellationTokenSource _tokenSource;
        private readonly Func<Update, T?> _getT;
        private readonly Filter<T>? _filter;
        private bool disposedValue;

        protected AbstractChannel(
            UpdateType updateType,
            Func<Update, T?> getT,
            Filter<T>? filter)
        {
            _filter = filter;
            UpdateType = updateType;
            _getT = getT ?? throw new ArgumentNullException(nameof(getT));
            _channel = Channel.CreateBounded<Update>(new BoundedChannelOptions(1)
            {
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = true
            });
            _tokenSource = new CancellationTokenSource();
        }

        public UpdateType UpdateType { get; }

        public T? GetT(Update update) => _getT(update);

        public Update? Update { get; private set; }

        public bool Cancelled => _tokenSource.IsCancellationRequested;

        public async Task<Update> ReadAsync(TimeSpan timeOut)
        {
            _tokenSource.CancelAfter(timeOut);
            Update = await _channel.Reader.ReadAsync(_tokenSource.Token);
            return Update;
        }

        public async Task WriteAsync(Update update)
            => await _channel.Writer.WriteAsync(update, _tokenSource.Token);

        protected bool ShouldChannel(T t)
        {
            if (_filter is null) return true;

            return _filter.TheyShellPass(t);
        }

        public bool ShouldChannel(Update update)
        {
            var insider = GetT(update);

            if (insider == null) return false;

            return ShouldChannel(insider);
        }

        public void Cancel()
        {
            _tokenSource.Cancel();
        }

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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
