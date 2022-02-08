using Telegram.Bot.Types.Enums;
using TelegramUpdater.Asp.Services;

namespace WebhookApp.Services
{
    public class WebhookConfigs : ConfigureWebhook
    {
        public WebhookConfigs(ILogger<ConfigureWebhook> logger, IServiceProvider serviceProvider, IConfiguration configuration) : base(logger, serviceProvider, configuration)
        {
        }

        protected override UpdateType[]? AllowedUpdates
            => new[] { UpdateType.CallbackQuery, UpdateType.Message };

        protected override string WebhookAddress
            => $@"{UpdaterConfigs.HostAddress}/updates/{UpdaterConfigs.BotToken}";
    }
}
