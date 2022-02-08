using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using TelegramUpdater.UpdateHandlers.ScopedHandlers;

namespace TelegramUpdater.Hosting
{
    public static class UpdaterServiceExtensions
    {
        /// <summary>
        /// Adds an <see cref="ITelegramBotClient"/> to the service collection.
        /// </summary>
        public static void AddTelegramBotClient(this IServiceCollection services,
                                                string botToken)
        {
            // Register named HttpClient to get benefits of IHttpClientFactory
            // and consume it with ITelegramBotClient typed client.
            // More read:
            //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
            //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
            services.AddHttpClient("tgwebhook")
                    .AddTypedClient<ITelegramBotClient>(httpClient
                        => new TelegramBotClient(botToken, httpClient));
        }

        /// <summary>
        /// Add telegram updater to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="configs">
        /// You can use <see cref="AspExtensions.GetUpdaterConfigs(Microsoft.Extensions.Configuration.IConfiguration, string)"/>
        /// to read configs from appsettings.json
        /// </param>
        public static void AddTelegramUpdater(this IServiceCollection serviceDescriptors,
                                              UpdaterConfigs configs,
                                              Action<UpdaterServiceBuilder> builder)
        {
            serviceDescriptors.AddTelegramBotClient(configs.BotToken);

            var updaterBuilder = new UpdaterServiceBuilder();
            builder(updaterBuilder);

            foreach (var container in updaterBuilder.IterScopedContainers())
            {
                serviceDescriptors.AddScoped(container.ScopedHandlerType);
            }

            serviceDescriptors.AddSingleton(
                services =>
                {
                    var botClient = services.GetRequiredService<ITelegramBotClient>();
                    var updater = new Updater(botClient, new UpdaterOptions(
                        configs.MaxDegreeOfParallelism,
                        configs.PerUserOneByOneProcess,
                        logger: services.GetRequiredService<ILogger<Updater>>()
                     ), services);

                    updaterBuilder.AddToUpdater(updater);
                    return updater;
                });

            serviceDescriptors.AddHostedService<UpdaterService>();
        }

        /// <summary>
        /// Add telegram updater to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="configs">
        /// You can use <see cref="AspExtensions.GetUpdaterConfigs(Microsoft.Extensions.Configuration.IConfiguration, string)"/>
        /// to read configs from appsettings.json
        /// </param>
        public static void AddTelegramUpdater<T>(this IServiceCollection serviceDescriptors,
                                                 UpdaterConfigs configs,
                                                 Action<UpdaterServiceBuilder> builder)
            where T: UpdaterService
        {
            serviceDescriptors.AddTelegramBotClient(configs.BotToken);

            var updaterBuilder = new UpdaterServiceBuilder();
            builder(updaterBuilder);

            foreach (var container in updaterBuilder.IterScopedContainers())
            {
                serviceDescriptors.AddScoped(container.ScopedHandlerType);
            }

            serviceDescriptors.AddSingleton(
                services =>
                {
                    var botClient = services.GetRequiredService<ITelegramBotClient>();
                    var updater = new Updater(botClient, new UpdaterOptions(
                        configs.MaxDegreeOfParallelism,
                        configs.PerUserOneByOneProcess,
                        logger: services.GetRequiredService<ILogger<Updater>>()
                     ), services);

                    updaterBuilder.AddToUpdater(updater);
                    return updater;
                });

            serviceDescriptors.AddHostedService<T>();
        }

        /// <summary>
        /// Add telegram updater to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// This method will also adds <see cref="ITelegramBotClient"/> to the service collection as Singleton.
        /// If you already did that, Pass an instance of <see cref="ITelegramBotClient"/> instead of <paramref name="botToken"/>.
        /// </remarks>
        /// <param name="botToken">Your bot api token.</param>
        public static void AddTelegramUpdater(this IServiceCollection serviceDescriptors,
                                              string botToken,
                                              UpdaterOptions updaterOptions,
                                              Action<UpdaterServiceBuilder> builder)
        {
            serviceDescriptors.AddTelegramBotClient(botToken);

            var updaterBuilder = new UpdaterServiceBuilder();
            builder(updaterBuilder);

            foreach (var container in updaterBuilder.IterScopedContainers())
            {
                serviceDescriptors.AddScoped(container.ScopedHandlerType);
            }

            serviceDescriptors.AddSingleton(
                services =>
                {
                    var botClient = services.GetRequiredService<ITelegramBotClient>();
                    var updater = new Updater(botClient, new UpdaterOptions(
                        updaterOptions.MaxDegreeOfParallelism,
                        updaterOptions.PerUserOneByOneProcess,
                        logger: services.GetRequiredService<ILogger<Updater>>(),
                        updaterOptions.CancellationToken,
                        updaterOptions.FlushUpdatesQueue,
                        updaterOptions.AllowedUpdates
                     ), services);

                    updaterBuilder.AddToUpdater(updater);
                    return updater;
                });

            serviceDescriptors.AddHostedService<UpdaterService>();
        }

        /// <summary>
        /// Add telegram updater to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// This method will also adds <see cref="ITelegramBotClient"/> to the service collection as Singleton.
        /// If you already did that, Pass an instance of <see cref="ITelegramBotClient"/> instead of <paramref name="botToken"/>.
        /// </remarks>
        /// <param name="botToken">Your bot api token.</param>
        public static void AddTelegramUpdater<T>(this IServiceCollection serviceDescriptors,
                                                 string botToken,
                                                 UpdaterOptions updaterOptions,
                                                 Action<UpdaterServiceBuilder> builder)
            where T : UpdaterService
        {
            serviceDescriptors.AddTelegramBotClient(botToken);

            var updaterBuilder = new UpdaterServiceBuilder();
            builder(updaterBuilder);

            foreach (var container in updaterBuilder.IterScopedContainers())
            {
                serviceDescriptors.AddScoped(container.ScopedHandlerType);
            }

            serviceDescriptors.AddSingleton(
                services =>
                {
                    var botClient = services.GetRequiredService<ITelegramBotClient>();
                    var updater = new Updater(botClient, new UpdaterOptions(
                        updaterOptions.MaxDegreeOfParallelism,
                        updaterOptions.PerUserOneByOneProcess,
                        logger: services.GetRequiredService<ILogger<Updater>>(),
                        updaterOptions.CancellationToken,
                        updaterOptions.FlushUpdatesQueue,
                        updaterOptions.AllowedUpdates
                     ), services);

                    updaterBuilder.AddToUpdater(updater);
                    return updater;
                });

            serviceDescriptors.AddHostedService<T>();
        }

        /// <summary>
        /// Add telegram updater to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// This method adds updater and handlers to the <see cref="IServiceCollection"/>,
        /// But not <paramref name="telegramBot"/>! and you should do it yourself.
        /// <para>You better pass botToken as <see cref="string"/></para>
        /// </remarks>
        /// <param name="telegramBot"><see cref="ITelegramBotClient"/> required by <see cref="Updater"/>.</param>
        public static void AddTelegramUpdater(this IServiceCollection serviceDescriptors,
                                              ITelegramBotClient telegramBot,
                                              UpdaterOptions updaterOptions,
                                              Action<UpdaterServiceBuilder> builder)
        {
            var updaterBuilder = new UpdaterServiceBuilder();
            builder(updaterBuilder);

            foreach (var container in updaterBuilder.IterScopedContainers())
            {
                serviceDescriptors.AddScoped(container.ScopedHandlerType);
            }

            serviceDescriptors.AddSingleton(
                services =>
                {
                    var updater = new Updater(telegramBot, new UpdaterOptions(
                        updaterOptions.MaxDegreeOfParallelism,
                        updaterOptions.PerUserOneByOneProcess,
                        logger: services.GetRequiredService<ILogger<Updater>>(),
                        updaterOptions.CancellationToken,
                        updaterOptions.FlushUpdatesQueue,
                        updaterOptions.AllowedUpdates
                     ), services);

                    updaterBuilder.AddToUpdater(updater);
                    return updater;
                });

            serviceDescriptors.AddHostedService<UpdaterService>();
        }

        /// <summary>
        /// Add telegram updater to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// This method adds updater and handlers to the <see cref="IServiceCollection"/>,
        /// But not <paramref name="telegramBot"/>! and you should do it yourself.
        /// <para>You better pass botToken as <see cref="string"/></para>
        /// </remarks>
        /// <param name="telegramBot"><see cref="ITelegramBotClient"/> required by <see cref="Updater"/>.</param>
        public static void AddTelegramUpdater<T>(this IServiceCollection serviceDescriptors,
                                              ITelegramBotClient telegramBot,
                                              UpdaterOptions updaterOptions,
                                              Action<UpdaterServiceBuilder> builder)
            where T : UpdaterService
        {
            var updaterBuilder = new UpdaterServiceBuilder();
            builder(updaterBuilder);

            foreach (var container in updaterBuilder.IterScopedContainers())
            {
                serviceDescriptors.AddScoped(container.ScopedHandlerType);
            }

            serviceDescriptors.AddSingleton(
                services =>
                {
                    var updater = new Updater(telegramBot, new UpdaterOptions(
                        updaterOptions.MaxDegreeOfParallelism,
                        updaterOptions.PerUserOneByOneProcess,
                        logger: services.GetRequiredService<ILogger<Updater>>(),
                        updaterOptions.CancellationToken,
                        updaterOptions.FlushUpdatesQueue,
                        updaterOptions.AllowedUpdates
                     ), services);

                    updaterBuilder.AddToUpdater(updater);
                    return updater;
                });

            serviceDescriptors.AddHostedService<T>();
        }

        public static Updater RegisterUpdateHandler<THandler, TUpdate>(
            this Updater updater,
            Filter<TUpdate>? filter = default)
            where THandler : class, IScopedUpdateHandler where TUpdate : class
        {
            updater.AddScopedHandler<THandler, TUpdate>(filter);
            return updater;
        }
    }
}
