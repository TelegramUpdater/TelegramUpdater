using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramUpdater.Hosting;

/// <summary>
/// A set of hosting extensions for <see cref="IUpdater"/>. 
/// </summary>
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
    /// Using your custom update writer service.
    /// </summary>
    /// <remarks>
    /// This method will also adds <see cref="ITelegramBotClient"/> to the service collection as Singleton.
    /// If you already did that, Pass an instance of <see cref="ITelegramBotClient"/> instead of <paramref name="botToken"/>.
    /// <para>
    /// <paramref name="preUpdateProcessorType"/> will be added to services if it's available.
    /// </para>
    /// </remarks>
    /// <param name="botToken">Your bot api token.</param>
    /// <param name="updaterOptions">Updater options.</param>
    /// <param name="serviceDescriptors">The service collection.</param>
    /// <param name="preUpdateProcessorType">
    /// Type of a class that will be initialized on every incoming update.
    /// It should be a sub-class of <see cref="AbstractPreUpdateProcessor"/>.
    /// <para>
    /// Your class should have a parameterless ctor if
    /// <paramref name="serviceDescriptors"/>
    /// is <see langword="null"/>.
    /// otherwise you can use items which are in services.
    /// </para>
    /// <para>
    /// Don't forget to add this to service collections if available.
    /// </para>
    /// </param>
    /// <param name="builder">Use this to config your <see cref="IUpdater"/>.</param>
    /// <typeparam name="TWriter">Type of your custom updater service. a child class of <see cref="UpdateWriterServiceAbs"/></typeparam>
    public static void AddTelegramUpdater<TWriter>(this IServiceCollection serviceDescriptors,
                                                   string botToken,
                                                   UpdaterOptions updaterOptions = default,
                                                   Action<UpdaterServiceBuilder>? builder = default,
                                                   Type? preUpdateProcessorType = default)
        where TWriter : UpdateWriterServiceAbs
    {
        serviceDescriptors.AddTelegramBotClient(botToken);

        var updaterBuilder = new UpdaterServiceBuilder();
        builder?.Invoke(updaterBuilder);

        updaterBuilder.AddToServiceCollection(serviceDescriptors);

        serviceDescriptors.AddSingleton<IUpdater>(
            services =>
            {
                var botClient = services.GetRequiredService<ITelegramBotClient>();
                var updater = new Updater(botClient, new UpdaterOptions(
                    updaterOptions.MaxDegreeOfParallelism,
                    logger: services.GetRequiredService<ILogger<IUpdater>>(),
                    updaterOptions.CancellationToken,
                    updaterOptions.FlushUpdatesQueue,
                    updaterOptions.AllowedUpdates
                 ), services, preUpdateProcessorType);

                updaterBuilder.AddToUpdater(updater);
                return updater;
            });

        if (preUpdateProcessorType is not null)
        {
            serviceDescriptors.AddScoped(preUpdateProcessorType);
        }
        serviceDescriptors.AddHostedService<TWriter>();
    }

    /// <summary>
    /// Add telegram updater to the <see cref="IServiceCollection"/>.
    /// Using your custom update writer service.
    /// </summary>
    /// <remarks>
    /// This method adds updater and handlers to the <see cref="IServiceCollection"/>,
    /// But not <paramref name="telegramBot"/>! and you should do it yourself.
    /// <para>You better pass botToken as <see cref="string"/></para>
    /// <para>
    /// <paramref name="preUpdateProcessorType"/> will be added to services if it's available.
    /// </para>
    /// </remarks>
    /// <param name="serviceDescriptors">The service collection.</param>
    /// <param name="telegramBot"><see cref="ITelegramBotClient"/> required by <see cref="Updater"/>.</param>
    /// <param name="preUpdateProcessorType">
    /// Type of a class that will be initialized on every incoming update.
    /// It should be a sub-class of <see cref="AbstractPreUpdateProcessor"/>.
    /// <para>
    /// Your class should have a parameterless ctor if
    /// <paramref name="serviceDescriptors"/>
    /// is <see langword="null"/>.
    /// otherwise you can use items which are in services.
    /// </para>
    /// <para>
    /// Don't forget to add this to service collections if available.
    /// </para>
    /// </param>
    /// <param name="updaterOptions">Options for this updater.</param>
    /// <param name="builder">Use this to config your <see cref="IUpdater"/>.</param>
    /// <typeparam name="TWriter">Type of your custom updater service. a child class of <see cref="UpdateWriterServiceAbs"/></typeparam>
    public static void AddTelegramUpdater<TWriter>(this IServiceCollection serviceDescriptors,
                                                   ITelegramBotClient telegramBot,
                                                   UpdaterOptions updaterOptions = default,
                                                   Action<UpdaterServiceBuilder>? builder = default,
                                                   Type? preUpdateProcessorType = default)
        where TWriter : UpdateWriterServiceAbs
    {
        var updaterBuilder = new UpdaterServiceBuilder();
        builder?.Invoke(updaterBuilder);

        updaterBuilder.AddToServiceCollection(serviceDescriptors);

        serviceDescriptors.AddSingleton<IUpdater>(
            services =>
            {
                var updater = new Updater(telegramBot, new UpdaterOptions(
                    updaterOptions.MaxDegreeOfParallelism,
                    logger: services.GetRequiredService<ILogger<IUpdater>>(),
                    updaterOptions.CancellationToken,
                    updaterOptions.FlushUpdatesQueue,
                    updaterOptions.AllowedUpdates
                 ), services, preUpdateProcessorType);

                updaterBuilder.AddToUpdater(updater);
                return updater;
            });

        if (preUpdateProcessorType is not null)
        {
            serviceDescriptors.AddScoped(preUpdateProcessorType);
        }
        serviceDescriptors.AddHostedService<TWriter>();
    }

    /// <summary>
    /// Add telegram updater to the <see cref="IServiceCollection"/>.
    /// Using your custom update writer service.
    /// </summary>
    /// <remarks>
    /// This method adds updater and handlers to the <see cref="IServiceCollection"/>.
    /// <para>This method gets <see cref="ITelegramBotClient"/> from services.</para>
    /// <para>
    /// <paramref name="preUpdateProcessorType"/> will be added to services if it's available.
    /// </para>
    /// </remarks>
    /// <typeparam name="TWriter">Type of your custom updater service. a child class of <see cref="UpdateWriterServiceAbs"/></typeparam>
    public static void AddTelegramUpdater<TWriter>(this IServiceCollection serviceDescriptors,
                                                   UpdaterOptions updaterOptions,
                                                   Action<UpdaterServiceBuilder> builder,
                                                   Type? preUpdateProcessorType = default)
        where TWriter : UpdateWriterServiceAbs
    {
        var updaterBuilder = new UpdaterServiceBuilder();
        builder(updaterBuilder);

        updaterBuilder.AddToServiceCollection(serviceDescriptors);

        serviceDescriptors.AddSingleton<IUpdater>(
            services =>
            {
                var telegramBot = services.GetRequiredService<ITelegramBotClient>();

                var updater = new Updater(telegramBot, new UpdaterOptions(
                    updaterOptions.MaxDegreeOfParallelism,
                    logger: services.GetRequiredService<ILogger<IUpdater>>(),
                    updaterOptions.CancellationToken,
                    updaterOptions.FlushUpdatesQueue,
                    updaterOptions.AllowedUpdates
                 ), services, preUpdateProcessorType);

                updaterBuilder.AddToUpdater(updater);
                return updater;
            });

        if (preUpdateProcessorType is not null)
        {
            serviceDescriptors.AddScoped(preUpdateProcessorType);
        }
        serviceDescriptors.AddHostedService<TWriter>();
    }

    /// <summary>
    /// Add telegram updater to the <see cref="IServiceCollection"/>.
    /// Using a simple update writer
    /// </summary>
    /// <remarks>
    /// This method will also adds <see cref="ITelegramBotClient"/> to the service collection as Singleton.
    /// If you already did that, Pass an instance of <see cref="ITelegramBotClient"/> instead of <paramref name="botToken"/>.
    /// <para>
    /// <paramref name="preUpdateProcessorType"/> will be added to services if it's available.
    /// </para>
    /// </remarks>
    /// <param name="botToken">Your bot api token.</param>
    /// <param name="updaterOptions">Updater options.</param>
    /// <param name="serviceDescriptors">The service collection.</param>
    /// <param name="preUpdateProcessorType">
    /// Type of a class that will be initialized on every incoming update.
    /// It should be a sub-class of <see cref="AbstractPreUpdateProcessor"/>.
    /// <para>
    /// Your class should have a parameterless ctor if
    /// <paramref name="serviceDescriptors"/>
    /// is <see langword="null"/>.
    /// otherwise you can use items which are in services.
    /// </para>
    /// <para>
    /// Don't forget to add this to service collections if available.
    /// </para>
    /// </param>
    /// <param name="builder">Use this to config your <see cref="IUpdater"/>.</param>
    public static void AddTelegramUpdater(this IServiceCollection serviceDescriptors,
                                          string botToken,
                                          UpdaterOptions updaterOptions = default,
                                          Action<UpdaterServiceBuilder>? builder = default,
                                          Type? preUpdateProcessorType = default)
    {
        serviceDescriptors.AddTelegramUpdater<SimpleWriterService>(
            botToken, updaterOptions, builder, preUpdateProcessorType);
    }

    /// <summary>
    /// Add telegram updater to the <see cref="IServiceCollection"/>.
    /// Using a simple update writer
    /// </summary>
    /// <remarks>
    /// This method adds updater and handlers to the <see cref="IServiceCollection"/>,
    /// But not <paramref name="telegramBot"/>! and you should do it yourself.
    /// <para>You better pass botToken as <see cref="string"/></para>
    /// <para>
    /// <paramref name="preUpdateProcessorType"/> will be added to services if it's available.
    /// </para>
    /// </remarks>
    /// <param name="telegramBot"><see cref="ITelegramBotClient"/> required by <see cref="Updater"/>.</param>
    /// <param name="serviceDescriptors">The service collection.</param>
    /// <param name="preUpdateProcessorType">
    /// Type of a class that will be initialized on every incoming update.
    /// It should be a sub-class of <see cref="AbstractPreUpdateProcessor"/>.
    /// <para>
    /// Your class should have a parameterless ctor if
    /// <paramref name="serviceDescriptors"/>
    /// is <see langword="null"/>.
    /// otherwise you can use items which are in services.
    /// </para>
    /// <para>
    /// Don't forget to add this to service collections if available.
    /// </para>
    /// </param>
    /// <param name="builder">Use this to config your <see cref="IUpdater"/>.</param>
    /// <param name="updaterOptions">Options for this updater.</param>
    public static void AddTelegramUpdater(this IServiceCollection serviceDescriptors,
                                          ITelegramBotClient telegramBot,
                                          UpdaterOptions updaterOptions = default,
                                          Action<UpdaterServiceBuilder>? builder = default,
                                          Type? preUpdateProcessorType = default)
    {
        serviceDescriptors.AddTelegramUpdater<SimpleWriterService>(
            telegramBot, updaterOptions, builder, preUpdateProcessorType);
    }

    /// <summary>
    /// Add telegram updater to the <see cref="IServiceCollection"/>.
    /// <b>Which the auto update writer is disabled!.</b>
    /// </summary>
    /// <remarks>
    /// This method adds updater and handlers to the <see cref="IServiceCollection"/>,
    /// But not <paramref name="telegramBot"/>! and you should do it yourself.
    /// <para>You better pass botToken as <see cref="string"/></para>
    /// <para>
    /// <paramref name="preUpdateProcessorType"/> will be added to services if it's available.
    /// </para>
    /// </remarks>
    /// <param name="telegramBot"><see cref="ITelegramBotClient"/> required by <see cref="Updater"/>.</param>
    /// <param name="serviceDescriptors">The service collection.</param>
    /// <param name="preUpdateProcessorType">
    /// Type of a class that will be initialized on every incoming update.
    /// It should be a sub-class of <see cref="AbstractPreUpdateProcessor"/>.
    /// <para>
    /// Your class should have a parameterless ctor if
    /// <paramref name="serviceDescriptors"/>
    /// is <see langword="null"/>.
    /// otherwise you can use items which are in services.
    /// </para>
    /// <para>
    /// Don't forget to add this to service collections if available.
    /// </para>
    /// </param>
    /// <param name="builder">Use this to config your <see cref="IUpdater"/>.</param>
    /// <param name="updaterOptions">Options for this updater.</param>
    public static void AddTelegramManualUpdater(this IServiceCollection serviceDescriptors,
                                                ITelegramBotClient telegramBot,
                                                UpdaterOptions updaterOptions = default,
                                                Action<UpdaterServiceBuilder>? builder = default,
                                                Type? preUpdateProcessorType = default)
    {
        var updaterBuilder = new UpdaterServiceBuilder();
        builder?.Invoke(updaterBuilder);

        updaterBuilder.AddToServiceCollection(serviceDescriptors);

        serviceDescriptors.AddSingleton<IUpdater>(
            services =>
            {
                var updater = new Updater(telegramBot, new UpdaterOptions(
                    updaterOptions.MaxDegreeOfParallelism,
                    logger: services.GetRequiredService<ILogger<IUpdater>>(),
                    updaterOptions.CancellationToken,
                    updaterOptions.FlushUpdatesQueue,
                    updaterOptions.AllowedUpdates
                 ), services, preUpdateProcessorType);

                updaterBuilder.AddToUpdater(updater);
                return updater;
            });

        if (preUpdateProcessorType is not null)
        {
            serviceDescriptors.AddScoped(preUpdateProcessorType);
        }
    }

    /// <summary>
    /// Add telegram updater to the <see cref="IServiceCollection"/>.
    /// <b>Which the auto update writer is disabled!.</b>
    /// </summary>
    /// <remarks>
    /// This method adds updater and handlers to the <see cref="IServiceCollection"/>,
    /// <para>This method gets <see cref="ITelegramBotClient"/> from services.</para>
    /// <para>
    /// <paramref name="preUpdateProcessorType"/> will be added to services if it's available.
    /// </para>
    /// </remarks>
    public static void AddTelegramManualUpdater(this IServiceCollection serviceDescriptors,
                                                UpdaterOptions updaterOptions,
                                                Action<UpdaterServiceBuilder> builder,
                                                Type? preUpdateProcessorType = default)
    {
        var updaterBuilder = new UpdaterServiceBuilder();
        builder(updaterBuilder);

        updaterBuilder.AddToServiceCollection(serviceDescriptors);

        serviceDescriptors.AddSingleton<IUpdater>(
            services =>
            {
                var telegramBot = services.GetRequiredService<ITelegramBotClient>();

                var updater = new Updater(telegramBot, new UpdaterOptions(
                    updaterOptions.MaxDegreeOfParallelism,
                    logger: services.GetRequiredService<ILogger<IUpdater>>(),
                    updaterOptions.CancellationToken,
                    updaterOptions.FlushUpdatesQueue,
                    updaterOptions.AllowedUpdates
                 ), services, preUpdateProcessorType);

                updaterBuilder.AddToUpdater(updater);
                return updater;
            });

        if (preUpdateProcessorType is not null)
        {
            serviceDescriptors.AddScoped(preUpdateProcessorType);
        }
    }

    /// <summary>
    /// Add telegram updater to the <see cref="IServiceCollection"/>.
    /// Using your custom update writer service.
    /// </summary>
    /// <remarks>
    /// This method will also adds <see cref="ITelegramBotClient"/> to the service collection as Singleton.
    /// <para>
    /// <paramref name="preUpdateProcessorType"/> will be added to services if it's available.
    /// </para>
    /// </remarks>
    /// <param name="botToken">Your bot api token.</param>
    /// <param name="updaterOptions">Updater options.</param>
    /// <param name="serviceDescriptors">The service collection.</param>
    /// <param name="preUpdateProcessorType">
    /// Type of a class that will be initialized on every incoming update.
    /// It should be a sub-class of <see cref="AbstractPreUpdateProcessor"/>.
    /// <para>
    /// Your class should have a parameterless ctor if
    /// <paramref name="serviceDescriptors"/>
    /// is <see langword="null"/>.
    /// otherwise you can use items which are in services.
    /// </para>
    /// <para>
    /// Don't forget to add this to service collections if available.
    /// </para>
    /// </param>
    /// <param name="builder">Use this to config your <see cref="IUpdater"/>.</param>
    public static void AddTelegramManualUpdater(this IServiceCollection serviceDescriptors,
                                                string botToken,
                                                UpdaterOptions updaterOptions = default,
                                                Action<UpdaterServiceBuilder>? builder = default,
                                                Type? preUpdateProcessorType = default)
    {
        serviceDescriptors.AddTelegramBotClient(botToken);

        var updaterBuilder = new UpdaterServiceBuilder();
        builder?.Invoke(updaterBuilder);

        updaterBuilder.AddToServiceCollection(serviceDescriptors);

        serviceDescriptors.AddSingleton<IUpdater>(
            services =>
            {
                var botClient = services.GetRequiredService<ITelegramBotClient>();
                var updater = new Updater(botClient, new UpdaterOptions(
                    updaterOptions.MaxDegreeOfParallelism,
                    logger: services.GetRequiredService<ILogger<IUpdater>>(),
                    updaterOptions.CancellationToken,
                    updaterOptions.FlushUpdatesQueue,
                    updaterOptions.AllowedUpdates
                 ), services, preUpdateProcessorType);

                updaterBuilder.AddToUpdater(updater);
                return updater;
            });

        if (preUpdateProcessorType is not null)
        {
            serviceDescriptors.AddScoped(preUpdateProcessorType);
        }
    }

    /// <summary>
    /// Use this method to write updates when manual writing in enabled.
    /// </summary>
    public static async Task WriteUpdateAsync(this IUpdater updater,
                                              Update update,
                                              CancellationToken cancellationToken = default)
        => await updater.WriteAsync(update, cancellationToken);
}
