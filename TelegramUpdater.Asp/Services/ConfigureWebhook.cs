using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.Asp.Services
{
    public class ConfigureWebhook : IHostedService
    {
        private readonly ILogger<ConfigureWebhook> _logger;
        private readonly IServiceProvider _services;
        protected readonly UpdaterConfigs _botConfig;

        public ConfigureWebhook(ILogger<ConfigureWebhook> logger,
                                IServiceProvider serviceProvider,
                                IConfiguration configuration)
        {
            _logger = logger;
            _services = serviceProvider;
            _botConfig = configuration.GetUpdaterConfigs();
        }

        protected UpdaterConfigs UpdaterConfigs => _botConfig;

        /// <summary>
        /// Defaults to <c>$"{<see cref="UpdaterConfigs.HostAddress"/>}/bot/{<see cref="UpdaterConfigs.BotToken"/>}"</c>
        /// <para>
        /// See <see href="https://core.telegram.org/bots/api#setwebhook"/>
        /// </para>
        /// </summary>
        protected virtual string WebhookAddress
            => @$"{_botConfig.HostAddress}/bot/{_botConfig.BotToken}";

        /// <summary>
        /// Updates you want to receive. Defaults to <c>Array.Empty()</c>
        /// </summary>
        protected virtual UpdateType[]? AllowedUpdates => Array.Empty<UpdateType>();

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            // Configure custom endpoint per Telegram API recommendations:
            // https://core.telegram.org/bots/api#setwebhook
            // If you'd like to make sure that the Webhook request comes from Telegram, we recommend
            // using a secret path in the URL, e.g. https://www.example.com/<token>.
            // Since nobody else knows your bot's token, you can be pretty sure it's us.
            _logger.LogInformation("Setting webhook: {webhookAddress}", WebhookAddress);
            await botClient.SetWebhookAsync(
                url: WebhookAddress,
                allowedUpdates: AllowedUpdates,
                cancellationToken: cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            // Remove webhook upon app shutdown
            _logger.LogInformation("Removing webhook");
            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }
    }
}
