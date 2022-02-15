namespace TelegramUpdater.Hosting
{
    public class UpdaterConfigs
    {
        /// <summary>
        /// Maximum number of allowed concurent update handling tasks.
        /// </summary>
        public int? MaxDegreeOfParallelism { get; set; }

        public string? BotToken { get; set; }

        public string? HostAddress { get; set; }
    }
}
