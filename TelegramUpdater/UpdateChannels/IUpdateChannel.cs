using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.UpdateChannels
{
    public interface IUpdateChannel: IDisposable
    {
        public UpdateType UpdateType { get; }

        public Update? Update { get; }

        public bool ShouldChannel(Update update);

        public Task<Update> ReadAsync(TimeSpan timeOut);

        public Task WriteAsync(Update update);

        public void Cancel();

        public bool Cancelled { get; }
    }
}
