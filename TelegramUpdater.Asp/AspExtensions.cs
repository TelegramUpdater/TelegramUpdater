using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramUpdater.Asp.Services;
using TelegramUpdater.Hosting;

namespace TelegramUpdater.Asp
{
    public static class AspExtensions
    {
        /// <summary>
        /// Use this in you webhook app if you wanna write updates from webhook controller,
        /// Using <see cref="WriteUpdateFromWebhook(IUpdater, Update)"/>
        /// </summary>
        /// <param name="serviceDescriptors"></param>
        /// <param name="configs"></param>
        /// <param name="builder"></param>
        public static void AddTelegramManualUpdater(this IServiceCollection serviceDescriptors,
                                                    UpdaterConfigs configs,
                                                    Action<UpdaterServiceBuilder> builder)
        {
            serviceDescriptors.AddTelegramUpdater<ManualWritingUpdaterService>(configs, builder);
        }

        /// <summary>
        /// Gets updater configs from appsettings and <paramref name="sectionName"/> section.
        /// <para>- Example of that section:</para>
        /// <code>
        /// "UpdaterConfigs": {
        ///     "MaxDegreeOfParallelism": null,
        ///     "PerUserOneByOneProcess": true,
        ///     "BotToken": "BOT-TOKEN",
        ///     "HostAddress": "yourdomin.com"
        /// }
        /// </code>
        /// </summary>
        public static UpdaterConfigs GetUpdaterConfigs(
            this IConfiguration configuration,
            string sectionName = "UpdaterConfigs")
        {
            return configuration.GetSection(sectionName).Get<UpdaterConfigs>();
        }

        /// <summary>
        /// Configure webhook. Defaults to <b><c>hostAddress/bot/botToken</c></b> and all updates.
        /// </summary>
        /// <param name="services"></param>
        public static void AddWebhookConfigs(this IServiceCollection services)
        {
            // There are several strategies for completing asynchronous tasks during startup.
            // Some of them could be found in this article https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/
            // We are going to use IHostedService to add and later remove Webhook
            services.AddHostedService<ConfigureWebhook>();
        }

        /// <summary>
        /// Configure your custom webhook, Create a sub-class of <see cref="ConfigureWebhook"/>,
        /// And pass as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Custom webhook configure class</typeparam>
        public static void AddWebhookConfigs<T>(this IServiceCollection services)
            where T : ConfigureWebhook
        {
            // There are several strategies for completing asynchronous tasks during startup.
            // Some of them could be found in this article https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/
            // We are going to use IHostedService to add and later remove Webhook
            services.AddHostedService<T>();
        }

        /// <summary>
        /// Creates a route map for webhook updates from telegram
        /// <para>
        /// It's $"bot/{<see cref="UpdaterConfigs.BotToken"/>} by default, which is mapped to
        /// <c>Webhook</c> Controller.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para><b>NOTE:</b></para>
        /// You can use suitable <see cref="RouteAttribute"/> on your webhook controller
        /// and don't use this method at all.
        /// </remarks>
        /// <param name="name">Name of route mapping.</param>
        /// <param name="updaterConfigs">Updater configs.
        /// <para>Should not be null for default mapping.</para></param>
        /// <param name="pattern">Use this if you want a custom pattern</param>
        /// <param name="controllerName">You controller name to receive updates.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void MapWebhook(this IEndpointRouteBuilder endpoints,
                                      string name,
                                      UpdaterConfigs? updaterConfigs = default,
                                      string? pattern = default,
                                      string controllerName = "Webhook")
        {
            if (updaterConfigs != null)
            {
                pattern = $"bot/{updaterConfigs.BotToken}";
            }

            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException(nameof(pattern));

            // Configure custom endpoint per Telegram API recommendations:
            // https://core.telegram.org/bots/api#setwebhook
            // If you'd like to make sure that the Webhook request comes from Telegram, we recommend
            // using a secret path in the URL, e.g. https://www.example.com/<token>.
            // Since nobody else knows your bot's token, you can be pretty sure it's us.
            endpoints.MapControllerRoute(name: name,
                                         pattern: pattern,
                                         new { controller = controllerName, action = "Post" });
        }

        public static async Task<ActionResult> WriteUpdateFromWebhook(this IUpdater updater,
                                                                      Update update, 
                                                                      CancellationToken cancellationToken = default)
        {
            await updater.WriteAsync(update, cancellationToken);
            return new OkResult();
        }
    }
}
