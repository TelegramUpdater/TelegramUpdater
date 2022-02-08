namespace TelegramUpdater.Asp
{
    public class UpdaterConfigs
    {
        /// <summary>
        /// Maximum number of allowed concurent update handling tasks.
        /// </summary>
        public int? MaxDegreeOfParallelism { get; set; }

        /// <summary>
        /// User should wait for a request to finish to start a new one.
        /// </summary>
        public bool PerUserOneByOneProcess { get; set; }

        public string? BotToken { get; set; }

        public string? HostAddress { get; set; }
    }
}
