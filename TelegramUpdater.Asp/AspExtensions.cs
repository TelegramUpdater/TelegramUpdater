using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using TelegramUpdater.Asp.Services;

namespace TelegramUpdater.Asp
{
    public static class AspExtensions
    {
        public static UpdaterConfigs ReadUpdaterConfigs(
            this IConfiguration configuration,
            string sectionName = "UpdaterConfigs")
        {
            return configuration.GetSection(sectionName).Get<UpdaterConfigs>();
        }

        public static void AddWebhookConfigs(this IServiceCollection services)
        {
            // There are several strategies for completing asynchronous tasks during startup.
            // Some of them could be found in this article https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/
            // We are going to use IHostedService to add and later remove Webhook
            services.AddHostedService<ConfigureWebhook>();
        }

        public static void AddTelegramBotClient(this IServiceCollection services,
                                                UpdaterConfigs botConfigs)
        {
            // Register named HttpClient to get benefits of IHttpClientFactory
            // and consume it with ITelegramBotClient typed client.
            // More read:
            //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
            //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            services.AddHttpClient("tgwebhook")
                    .AddTypedClient<ITelegramBotClient>(httpClient
                        => new TelegramBotClient(botConfigs.BotToken, httpClient));
        }

        public static void MapWebhook(this IEndpointRouteBuilder endpoints,
                                      string name,
                                      UpdaterConfigs? botConfigs = default,
                                      string? pattern = default,
                                      string controllerName = "Webhook")
        {
            if (botConfigs != null)
            {
                pattern = $"bot/{botConfigs.BotToken}";
            }

            if (string.IsNullOrEmpty(pattern))
                throw new System.ArgumentNullException(nameof(pattern));

            // Configure custom endpoint per Telegram API recommendations:
            // https://core.telegram.org/bots/api#setwebhook
            // If you'd like to make sure that the Webhook request comes from Telegram, we recommend
            // using a secret path in the URL, e.g. https://www.example.com/<token>.
            // Since nobody else knows your bot's token, you can be pretty sure it's us.
            endpoints.MapControllerRoute(name: name,
                                         pattern: pattern,
                                         new { controller = controllerName, action = "Post" });
        }
    }
}
