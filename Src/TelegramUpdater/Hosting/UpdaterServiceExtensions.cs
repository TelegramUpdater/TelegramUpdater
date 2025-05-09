﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelegramUpdater.UpdateWriters;

namespace TelegramUpdater.Hosting;

/// <summary>
/// A set of hosting extensions for <see cref="IUpdater"/>. 
/// </summary>
public static class UpdaterServiceExtensions
{
    /// <summary>
    /// Adds an <see cref="ITelegramBotClient"/> to the service collection.
    /// </summary>
    public static void AddTelegramBotClient(
        this IServiceCollection services,
        string botToken) =>
        // Register named HttpClient to get benefits of IHttpClientFactory
        // and consume it with ITelegramBotClient typed client.
        // More read:
        //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
        //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
        services.AddHttpClient("tgwebhook")
                .RemoveAllLoggers()
                .AddTypedClient<ITelegramBotClient>(httpClient
                    => new TelegramBotClient(botToken, httpClient));

    private static void AddUpdaterWithBotClient(
        this IServiceCollection serviceDescriptors,
        Type? preUpdateProcessorType,
        UpdaterOptions? updaterOptions,
        UpdaterServiceBuilder updaterBuilder)
    {
        serviceDescriptors.AddSingleton<IUpdater>(
            (services) =>
            {
                var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();
                var botClient = services.GetRequiredService<ITelegramBotClient>();

                var updater = new Updater(
                    botClient: botClient,
                    updaterOptions: UpdaterExtensions.RedesignOptions(
                        updaterOptions: updaterOptions, serviceProvider: services),
                    scopeFactory: scopeFactory,
                    preUpdateProcessorType: preUpdateProcessorType);

                updaterBuilder.ApplyJobs(updater);
                return updater;
            });

        if (preUpdateProcessorType is not null)
        {
            serviceDescriptors.AddScoped(preUpdateProcessorType);
        }
    }

    private static void AddUpdater(
        this IServiceCollection serviceDescriptors,
        ITelegramBotClient botClient,
        UpdaterOptions? updaterOptions,
        Type? preUpdateProcessorType,
        UpdaterServiceBuilder updaterBuilder)
    {
        serviceDescriptors.AddSingleton<IUpdater>(
            services =>
            {
                var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();

                var updater = new Updater(
                    botClient: botClient,
                    updaterOptions: UpdaterExtensions.RedesignOptions(
                        updaterOptions: updaterOptions, serviceProvider: services),
                    scopeFactory: scopeFactory,
                    preUpdateProcessorType: preUpdateProcessorType);

                updaterBuilder.ApplyJobs(updater);
                return updater;
            });

        if (preUpdateProcessorType is not null)
        {
            serviceDescriptors.AddScoped(preUpdateProcessorType);
        }
    }

    private static IConfigurationSection? ConfigureUpdaterOptions(
        this IServiceCollection serviceDescriptors, IConfigurationManager configurationManager)
    {
        var updaterOptionsSection = configurationManager?.GetSection(UpdaterOptions.Updater);

        if (updaterOptionsSection is not null)
        {
            serviceDescriptors.Configure<UpdaterOptions>(updaterOptionsSection);
        }

        return updaterOptionsSection;
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
    /// <para>
    /// This method tries to resolve <see cref="UpdaterOptions"/> from configuration.
    /// </para>
    /// </remarks>
    /// <param name="serviceDescriptors">The service collection.</param>
    /// <param name="configurationManager"></param>
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
    /// <typeparam name="TWriter">Type of your custom updater service. a child class of <see cref="AbstractUpdateWriter"/></typeparam>
    public static IServiceCollection AddTelegramUpdater<TWriter>(
        this IServiceCollection serviceDescriptors,
        IConfigurationManager configurationManager,
        Action<UpdaterServiceBuilder>? builder = default,
        Type? preUpdateProcessorType = default)
        where TWriter : AbstractUpdateWriter
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(serviceDescriptors);
        ArgumentNullException.ThrowIfNull(configurationManager);
#else
        if (serviceDescriptors == null)
            throw new ArgumentNullException(nameof(serviceDescriptors));

        if (configurationManager == null)
            throw new ArgumentNullException(nameof(configurationManager));
#endif

        IConfigurationSection? updaterOptionsSection = serviceDescriptors.ConfigureUpdaterOptions(configurationManager);

        var updaterOptions = updaterOptionsSection?.Get<UpdaterOptions>() ??
                throw new InvalidOperationException("No configuration for UpdaterOptions found.");

        serviceDescriptors.AddTelegramBotClient(updaterOptions.BotToken ??
            throw new InvalidOperationException("No BotToken found in UpdaterOptions."));

        var updaterBuilder = new UpdaterServiceBuilder();
        builder?.Invoke(updaterBuilder);

        serviceDescriptors.AddUpdaterWithBotClient(preUpdateProcessorType, updaterOptions, updaterBuilder);
        updaterBuilder.AddToServiceCollection(serviceDescriptors);

        serviceDescriptors.AddSingleton<TWriter>();
        serviceDescriptors.AddHostedService<UpdateWriterService<TWriter>>();

        return serviceDescriptors;
    }

    /// <inheritdoc cref="AddTelegramUpdater{TWriter}(IServiceCollection, IConfigurationManager?, Action{UpdaterServiceBuilder}?, Type?)"/>
    public static IServiceCollection AddTelegramUpdater<TWriter>(
        this IHostApplicationBuilder applicationBuilder,
        Action<UpdaterServiceBuilder>? builder = default,
        Type? preUpdateProcessorType = default)
        where TWriter : AbstractUpdateWriter
        => AddTelegramUpdater<TWriter>(
            serviceDescriptors: applicationBuilder.Services,
            configurationManager: applicationBuilder.Configuration,
            builder: builder,
            preUpdateProcessorType: preUpdateProcessorType);

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
    /// <param name="botToken">Your bot API token.</param>
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
    /// <typeparam name="TWriter">Type of your custom updater service. a child class of <see cref="AbstractUpdateWriter"/></typeparam>
    public static IServiceCollection AddTelegramUpdater<TWriter>(
        this IServiceCollection serviceDescriptors,
        string botToken,
        UpdaterOptions? updaterOptions = default,
        Action<UpdaterServiceBuilder>? builder = default,
        Type? preUpdateProcessorType = default)
        where TWriter : AbstractUpdateWriter
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(serviceDescriptors);
#else
        if (serviceDescriptors == null)
            throw new ArgumentNullException(nameof(serviceDescriptors));
#endif

#if NET8_0_OR_GREATER
        ArgumentException.ThrowIfNullOrEmpty(botToken);
#else
        if (string.IsNullOrEmpty(botToken))
            throw new ArgumentNullException(nameof(serviceDescriptors));
#endif

        serviceDescriptors.AddTelegramBotClient(botToken);

        var updaterBuilder = new UpdaterServiceBuilder();
        builder?.Invoke(updaterBuilder);

        serviceDescriptors.AddUpdaterWithBotClient(preUpdateProcessorType, updaterOptions, updaterBuilder);
        updaterBuilder.AddToServiceCollection(serviceDescriptors);

        serviceDescriptors.AddSingleton<TWriter>();
        serviceDescriptors.AddHostedService<UpdateWriterService<TWriter>>();

        return serviceDescriptors;
    }

    /// <summary>
    /// Add telegram updater to the <see cref="IServiceCollection"/>.
    /// Using your custom update writer service.
    /// </summary>
    /// <remarks>
    /// This method adds updater and handlers to the <see cref="IServiceCollection"/>,
    /// But not <paramref name="botClient"/>! and you should do it yourself.
    /// <para>You better pass botToken as <see cref="string"/></para>
    /// <para>
    /// <paramref name="preUpdateProcessorType"/> will be added to services if it's available.
    /// </para>
    /// </remarks>
    /// <param name="serviceDescriptors">The service collection.</param>
    /// <param name="botClient"><see cref="ITelegramBotClient"/> required by <see cref="Updater"/>.</param>
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
    /// <typeparam name="TWriter">Type of your custom updater. a child class of <see cref="AbstractUpdateWriter"/></typeparam>
    public static IServiceCollection AddTelegramUpdater<TWriter>(
        this IServiceCollection serviceDescriptors,
        ITelegramBotClient botClient,
        UpdaterOptions? updaterOptions = default,
        Action<UpdaterServiceBuilder>? builder = default,
        Type? preUpdateProcessorType = default)
        where TWriter : AbstractUpdateWriter
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(serviceDescriptors);
        ArgumentNullException.ThrowIfNull(botClient);
#else
        if (serviceDescriptors == null)
            throw new ArgumentNullException(nameof(serviceDescriptors));

        if (botClient == null)
            throw new ArgumentNullException(nameof(botClient));
#endif

        var updaterBuilder = new UpdaterServiceBuilder();
        builder?.Invoke(updaterBuilder);

        serviceDescriptors.AddUpdater(botClient, updaterOptions, preUpdateProcessorType, updaterBuilder);
        updaterBuilder.AddToServiceCollection(serviceDescriptors);

        serviceDescriptors.AddSingleton<TWriter>();
        serviceDescriptors.AddHostedService<UpdateWriterService<TWriter>>();

        return serviceDescriptors;
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
    /// <typeparam name="TWriter">Type of your custom updater service. a child class of <see cref="AbstractUpdateWriter"/></typeparam>
    public static IServiceCollection AddTelegramUpdater<TWriter>(
        this IServiceCollection serviceDescriptors,
        UpdaterOptions updaterOptions,
        Action<UpdaterServiceBuilder> builder,
        Type? preUpdateProcessorType = default)
        where TWriter : AbstractUpdateWriter
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(serviceDescriptors);
#else
        if (serviceDescriptors == null)
            throw new ArgumentNullException(nameof(serviceDescriptors));
#endif

        var updaterBuilder = new UpdaterServiceBuilder();
        builder(updaterBuilder);

        serviceDescriptors.AddUpdaterWithBotClient(preUpdateProcessorType, updaterOptions, updaterBuilder);
        updaterBuilder.AddToServiceCollection(serviceDescriptors);

        serviceDescriptors.AddSingleton<TWriter>();
        serviceDescriptors.AddHostedService<UpdateWriterService<TWriter>>();

        return serviceDescriptors;
    }

    /// <inheritdoc cref="AddTelegramUpdater{TWriter}(IServiceCollection, string, UpdaterOptions?, Action{UpdaterServiceBuilder}?, Type?)"/>
    public static IServiceCollection AddTelegramUpdater(
        this IServiceCollection serviceDescriptors,
        string botToken,
        UpdaterOptions? updaterOptions = default,
        Action<UpdaterServiceBuilder>? builder = default,
        Type? preUpdateProcessorType = default)
        => serviceDescriptors.AddTelegramUpdater<DefaultUpdateWriter>(
            botToken, updaterOptions, builder, preUpdateProcessorType);

    /// <inheritdoc cref="AddTelegramUpdater{TWriter}(IServiceCollection, ITelegramBotClient, UpdaterOptions?, Action{UpdaterServiceBuilder}?, Type?)"/>
    public static IServiceCollection AddTelegramUpdater(
        this IServiceCollection serviceDescriptors,
        ITelegramBotClient telegramBot,
        UpdaterOptions? updaterOptions = default,
        Action<UpdaterServiceBuilder>? builder = default,
        Type? preUpdateProcessorType = default)
        => serviceDescriptors.AddTelegramUpdater<DefaultUpdateWriter>(
            telegramBot, updaterOptions, builder, preUpdateProcessorType);


    /// <inheritdoc cref="AddTelegramUpdater{TWriter}(IServiceCollection, IConfigurationManager?, Action{UpdaterServiceBuilder}?, Type?)"/>
    public static IServiceCollection AddTelegramUpdater(
        this IHostApplicationBuilder applicationBuilder,
        Action<UpdaterServiceBuilder>? builder = default,
        Type? preUpdateProcessorType = default)
        => AddTelegramUpdater<DefaultUpdateWriter>(
            serviceDescriptors: applicationBuilder.Services,
            configurationManager: applicationBuilder.Configuration,
            builder: builder,
            preUpdateProcessorType: preUpdateProcessorType);

    /// <inheritdoc cref="AddTelegramUpdater{TWriter}(IServiceCollection, IConfigurationManager?, Action{UpdaterServiceBuilder}?, Type?)"/>
    public static IServiceCollection AddTelegramUpdater(
        this IServiceCollection serviceDescriptors,
        IConfigurationManager configurationManager,
        Action<UpdaterServiceBuilder>? builder = default,
        Type? preUpdateProcessorType = default)
        => AddTelegramUpdater<DefaultUpdateWriter>(
            serviceDescriptors: serviceDescriptors,
            configurationManager: configurationManager,
            builder: builder,
            preUpdateProcessorType: preUpdateProcessorType);

    /// <summary>
    /// Add telegram updater to the <see cref="IServiceCollection"/>.
    /// <b>Which the auto update writer is disabled!.</b>
    /// </summary>
    /// <remarks>
    /// This method adds updater and handlers to the <see cref="IServiceCollection"/>,
    /// But not <paramref name="botClient"/>! and you should do it yourself.
    /// <para>You better pass botToken as <see cref="string"/></para>
    /// <para>
    /// <paramref name="preUpdateProcessorType"/> will be added to services if it's available.
    /// </para>
    /// </remarks>
    /// <param name="botClient"><see cref="ITelegramBotClient"/> required by <see cref="Updater"/>.</param>
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
    public static IServiceCollection AddTelegramManualUpdater(
        this IServiceCollection serviceDescriptors,
        ITelegramBotClient botClient,
        UpdaterOptions? updaterOptions = default,
        Action<UpdaterServiceBuilder>? builder = default,
        Type? preUpdateProcessorType = default)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(serviceDescriptors);
        ArgumentNullException.ThrowIfNull(botClient);
#else
        if (serviceDescriptors == null)
            throw new ArgumentNullException(nameof(serviceDescriptors));

        if (botClient == null)
            throw new ArgumentNullException(nameof(botClient));
#endif

        var updaterBuilder = new UpdaterServiceBuilder();
        builder?.Invoke(updaterBuilder);

        serviceDescriptors.AddUpdater(botClient, updaterOptions, preUpdateProcessorType, updaterBuilder);
        updaterBuilder.AddToServiceCollection(serviceDescriptors);

        return serviceDescriptors;
    }

    /// <summary>
    /// Add telegram updater to the <see cref="IServiceCollection"/>.
    /// <b>Which the auto update writer is disabled!.</b>
    /// </summary>
    /// <remarks>
    /// This method adds updater and handlers to the <see cref="IServiceCollection"/>,
    /// <para>You better pass botToken as <see cref="string"/></para>
    /// <para>
    /// <paramref name="preUpdateProcessorType"/> will be added to services if it's available.
    /// </para>
    /// </remarks>
    /// <param name="serviceDescriptors">The service collection.</param>
    /// <param name="configurationManager"></param>
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
    public static IServiceCollection AddTelegramManualUpdater(
        this IServiceCollection serviceDescriptors,
        IConfigurationManager configurationManager,
        Action<UpdaterServiceBuilder>? builder = default,
        Type? preUpdateProcessorType = default)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(serviceDescriptors);
        ArgumentNullException.ThrowIfNull(configurationManager);
#else
        if (serviceDescriptors == null)
            throw new ArgumentNullException(nameof(serviceDescriptors));

        if (configurationManager == null)
            throw new ArgumentNullException(nameof(configurationManager));
#endif
        IConfigurationSection? updaterOptionsSection = serviceDescriptors.ConfigureUpdaterOptions(configurationManager);

        var updaterOptions = updaterOptionsSection?.Get<UpdaterOptions>() ??
                throw new InvalidOperationException("No configuration for UpdaterOptions found.");

        serviceDescriptors.AddTelegramBotClient(updaterOptions.BotToken ??
            throw new InvalidOperationException("No BotToken found in UpdaterOptions."));

        return serviceDescriptors.AddTelegramManualUpdater(updaterOptions, builder, preUpdateProcessorType);
    }

    /// <summary>
    /// Add telegram updater to the <see cref="IServiceCollection"/>.
    /// <b>Which the auto update writer is disabled!.</b>
    /// </summary>
    /// <remarks>
    /// This method adds updater and handlers to the <see cref="IServiceCollection"/>,
    /// <para>
    /// <paramref name="preUpdateProcessorType"/> will be added to services if it's available.
    /// </para>
    /// </remarks>
    /// <param name="hostApplicationBuilder"></param>
    /// <param name="preUpdateProcessorType">
    /// Type of a class that will be initialized on every incoming update.
    /// It should be a sub-class of <see cref="AbstractPreUpdateProcessor"/>.
    /// <para>
    /// Your class should have a parameterless ctor if
    /// <see cref="IHostApplicationBuilder.Services"/>
    /// is <see langword="null"/>.
    /// otherwise you can use items which are in services.
    /// </para>
    /// <para>
    /// Don't forget to add this to service collections if available.
    /// </para>
    /// </param>
    /// <param name="builder">Use this to config your <see cref="IUpdater"/>.</param>
    public static IServiceCollection AddTelegramManualUpdater(
        this IHostApplicationBuilder hostApplicationBuilder,
        Action<UpdaterServiceBuilder>? builder = default,
        Type? preUpdateProcessorType = default)
    {
        return hostApplicationBuilder.Services.AddTelegramManualUpdater(
            hostApplicationBuilder.Configuration, builder, preUpdateProcessorType);
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
    public static IServiceCollection AddTelegramManualUpdater(
        this IServiceCollection serviceDescriptors,
        UpdaterOptions updaterOptions,
        Action<UpdaterServiceBuilder>? builder,
        Type? preUpdateProcessorType = default)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(serviceDescriptors);
#else
        if (serviceDescriptors == null)
            throw new ArgumentNullException(nameof(serviceDescriptors));
#endif

        var updaterBuilder = new UpdaterServiceBuilder();
        builder?.Invoke(updaterBuilder);

        serviceDescriptors.AddUpdaterWithBotClient(preUpdateProcessorType, updaterOptions, updaterBuilder);
        updaterBuilder.AddToServiceCollection(serviceDescriptors);

        return serviceDescriptors;
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
    /// <param name="botToken">Your bot API token.</param>
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
    public static IServiceCollection AddTelegramManualUpdater(
        this IServiceCollection serviceDescriptors,
        string botToken,
        UpdaterOptions? updaterOptions = default,
        Action<UpdaterServiceBuilder>? builder = default,
        Type? preUpdateProcessorType = default)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(serviceDescriptors);
#else
        if (serviceDescriptors == null)
            throw new ArgumentNullException(nameof(serviceDescriptors));
#endif

#if NET8_0_OR_GREATER
        ArgumentException.ThrowIfNullOrEmpty(botToken);
#else
        if (string.IsNullOrEmpty(botToken))
            throw new ArgumentNullException(nameof(serviceDescriptors));
#endif

        serviceDescriptors.AddTelegramBotClient(botToken);

        var updaterBuilder = new UpdaterServiceBuilder();
        builder?.Invoke(updaterBuilder);

        serviceDescriptors.AddUpdaterWithBotClient(preUpdateProcessorType, updaterOptions!, updaterBuilder);
        updaterBuilder.AddToServiceCollection(serviceDescriptors);

        return serviceDescriptors;
    }

    /// <summary>
    /// Use this method to write updates when manual writing in enabled.
    /// </summary>
    public static ValueTask WriteUpdate(
        this IUpdater updater,
        Update update,
        CancellationToken cancellationToken = default) => updater.Write(update, cancellationToken);
}
