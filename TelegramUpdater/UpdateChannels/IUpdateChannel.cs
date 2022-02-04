using Telegram.Bot.Types;

namespace TelegramUpdater.UpdateChannels
{
    public interface IUpdateChannel: IDisposable
    {
        public Update? Update { get; }

        public bool ShouldChannel(Update update);

        public Task<Update> ReadAsync(TimeSpan timeOut);

        public Task WriteAsync(Update update);

        public void Cancel();

        public bool Cancelled { get; }
    }
}
