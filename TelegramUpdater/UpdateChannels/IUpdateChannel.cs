using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.UpdateChannels
{
    /// <summary>
    /// Base interface for channels.
    /// </summary>
    public interface IUpdateChannel : IDisposable
    {
        /// <summary>
        /// Update type.
        /// </summary>
        public UpdateType UpdateType { get; }

        /// <summary>
        /// If this update should be channeled.
        /// </summary>
        public bool ShouldChannel(Update update);

        /// <summary>
        /// Write an update to channel.
        /// </summary>
        public Task WriteAsync(IUpdater updater, Update update);

        /// <summary>
        /// Cancel the channel.
        /// </summary>
        public void Cancel();

        /// <summary>
        /// If the channel is cancelled.
        /// </summary>
        public bool Cancelled { get; }
    }
}
