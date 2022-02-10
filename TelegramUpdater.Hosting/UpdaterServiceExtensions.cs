using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

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
        /// <remarks>
        /// Use updater configs only in webhook apps! Since it miss
        /// <see cref="UpdaterOptions.AllowedUpdates"/> and <see cref="UpdaterOptions.FlushUpdatesQueue"/>.
        /// If you wanna specify these and update writer is not an external webhook use <see cref="UpdaterOptions"/>.
        /// </remarks>
        public static void AddTelegramUpdater(this IServiceCollection serviceDescriptors,
                                              UpdaterConfigs configs,
                                              Action<UpdaterServiceBuilder> builder)
        {
            serviceDescriptors.AddTelegramBotClient(configs.BotToken);

            var updaterBuilder = new UpdaterServiceBuilder();
            builder(updaterBuilder);

            updaterBuilder.AddToServiceCollection(serviceDescriptors);

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
        /// <typeparam name="T">Type of your custom updater service. a child class of <see cref="UpdaterService"/></typeparam>
        /// <remarks>
        /// Use updater configs only in webhook apps! Since it miss
        /// <see cref="UpdaterOptions.AllowedUpdates"/> and <see cref="UpdaterOptions.FlushUpdatesQueue"/>.
        /// If you wanna specify these and update writer is not an external webhook use <see cref="UpdaterOptions"/>.
        /// </remarks>
        public static void AddTelegramUpdater<T>(this IServiceCollection serviceDescriptors,
                                                 UpdaterConfigs configs,
                                                 Action<UpdaterServiceBuilder> builder)
            where T : UpdaterService
        {
            serviceDescriptors.AddTelegramBotClient(configs.BotToken);

            var updaterBuilder = new UpdaterServiceBuilder();
            builder(updaterBuilder);

            updaterBuilder.AddToServiceCollection(serviceDescriptors);

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
        /// <param name="configs">
        /// You can use <see cref="AspExtensions.GetUpdaterConfigs(Microsoft.Extensions.Configuration.IConfiguration, string)"/>
        /// to read configs from appsettings.json
        /// </param>
        /// <typeparam name="TUpdater">Type of your custom updater service. a child class of <see cref="UpdaterService"/></typeparam>
        /// <typeparam name="TWriter">This is your writer background service. a child class of <see cref="UpdateWriterServiceAbs"/></typeparam>
        /// <remarks>
        /// Use updater configs only in webhook apps! Since it miss
        /// <see cref="UpdaterOptions.AllowedUpdates"/> and <see cref="UpdaterOptions.FlushUpdatesQueue"/>.
        /// If you wanna specify these and update writer is not an external webhook use <see cref="UpdaterOptions"/>.
        /// </remarks>
        public static void AddTelegramUpdater<TUpdater, TWriter>(this IServiceCollection serviceDescriptors,
                                                                 UpdaterConfigs configs,
                                                                 Action<UpdaterServiceBuilder> builder)
            where TUpdater : UpdaterService where TWriter : UpdateWriterServiceAbs
        {
            serviceDescriptors.AddTelegramBotClient(configs.BotToken);

            var updaterBuilder = new UpdaterServiceBuilder();
            builder(updaterBuilder);

            updaterBuilder.AddToServiceCollection(serviceDescriptors);

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

            serviceDescriptors.AddHostedService<TUpdater>();
            serviceDescriptors.AddHostedService<TWriter>();
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

            updaterBuilder.AddToServiceCollection(serviceDescriptors);

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

            serviceDescriptors.AddHostedService<ManualWritingUpdaterService>();
            serviceDescriptors.AddHostedService<WriterService>();
        }

        /// <summary>
        /// Add telegram updater to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>
        /// This method will also adds <see cref="ITelegramBotClient"/> to the service collection as Singleton.
        /// If you already did that, Pass an instance of <see cref="ITelegramBotClient"/> instead of <paramref name="botToken"/>.
        /// </remarks>
        /// <param name="botToken">Your bot api token.</param>
        /// <typeparam name="T">Type of your custom updater service. a child class of <see cref="UpdaterService"/></typeparam>
        public static void AddTelegramUpdater<T>(this IServiceCollection serviceDescriptors,
                                                 string botToken,
                                                 UpdaterOptions updaterOptions,
                                                 Action<UpdaterServiceBuilder> builder)
            where T : UpdaterService
        {
            serviceDescriptors.AddTelegramBotClient(botToken);

            var updaterBuilder = new UpdaterServiceBuilder();
            builder(updaterBuilder);

            updaterBuilder.AddToServiceCollection(serviceDescriptors);

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
        /// This method will also adds <see cref="ITelegramBotClient"/> to the service collection as Singleton.
        /// If you already did that, Pass an instance of <see cref="ITelegramBotClient"/> instead of <paramref name="botToken"/>.
        /// </remarks>
        /// <param name="botToken">Your bot api token.</param>
        /// <typeparam name="TWriter">This is your writer background service. a child class of <see cref="UpdateWriterServiceAbs"/></typeparam>
        public static void AddTelegramManualUpdater<TWriter>(this IServiceCollection serviceDescriptors,
                                                             string botToken,
                                                             UpdaterOptions updaterOptions,
                                                             Action<UpdaterServiceBuilder> builder)
            where TWriter : UpdateWriterServiceAbs
        {
            serviceDescriptors.AddTelegramBotClient(botToken);

            var updaterBuilder = new UpdaterServiceBuilder();
            builder(updaterBuilder);

            updaterBuilder.AddToServiceCollection(serviceDescriptors);

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

            serviceDescriptors.AddHostedService<ManualWritingUpdaterService>();
            serviceDescriptors.AddHostedService<TWriter>();
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

            updaterBuilder.AddToServiceCollection(serviceDescriptors);

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

            serviceDescriptors.AddHostedService<ManualWritingUpdaterService>();
            serviceDescriptors.AddHostedService<WriterService>();
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
        /// <typeparam name="T">Type of your custom updater service. a child class of <see cref="UpdaterService"/></typeparam>
        public static void AddTelegramUpdater<T>(this IServiceCollection serviceDescriptors,
                                                 ITelegramBotClient telegramBot,
                                                 UpdaterOptions updaterOptions,
                                                 Action<UpdaterServiceBuilder> builder)
            where T : UpdaterService
        {
            var updaterBuilder = new UpdaterServiceBuilder();
            builder(updaterBuilder);

            updaterBuilder.AddToServiceCollection(serviceDescriptors);

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

        /// <summary>
        /// Add telegram updater to the <see cref="IServiceCollection"/>.
        /// <b>Which the auto update writer is disabled!.</b>
        /// </summary>
        /// <remarks>
        /// This method adds updater and handlers to the <see cref="IServiceCollection"/>,
        /// But not <paramref name="telegramBot"/>! and you should do it yourself.
        /// <para>You better pass botToken as <see cref="string"/></para>
        /// </remarks>
        /// <typeparam name="TWriter">This is your writer background service. a child class of <see cref="UpdateWriterServiceAbs"/></typeparam>
        /// <param name="telegramBot"><see cref="ITelegramBotClient"/> required by <see cref="Updater"/>.</param>
        public static void AddTelegramManualUpdater<TWriter>(this IServiceCollection serviceDescriptors,
                                                             ITelegramBotClient telegramBot,
                                                             UpdaterOptions updaterOptions,
                                                             Action<UpdaterServiceBuilder> builder)
            where TWriter : UpdateWriterServiceAbs
        {
            var updaterBuilder = new UpdaterServiceBuilder();
            builder(updaterBuilder);

            updaterBuilder.AddToServiceCollection(serviceDescriptors);

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

            serviceDescriptors.AddHostedService<ManualWritingUpdaterService>();
            serviceDescriptors.AddHostedService<TWriter>();
        }

        /// <summary>
        /// Use this method to write updates when manual writing in enabled.
        /// </summary>
        public static async Task WriteUpdateAsync(this Updater updater,
                                                  Update update,
                                                  CancellationToken cancellationToken = default)
            => await updater.ChannelWriter.WriteAsync(update, cancellationToken);
    }
}
