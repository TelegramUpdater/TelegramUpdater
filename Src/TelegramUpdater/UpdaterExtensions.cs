// Ignore Spelling: Iter Webpage Webhook ip

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.Filters;
using TelegramUpdater.Helpers;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped;
using TelegramUpdater.UpdateHandlers.Scoped.Attributes;
using TelegramUpdater.UpdateHandlers.Singleton;
using TelegramUpdater.UpdateHandlers.Singleton.Attributes;
using TelegramUpdater.UpdateWriters;

namespace TelegramUpdater;

/// <summary>
/// Extension methods for <see cref="IUpdater"/>.
/// </summary>
public static class UpdaterExtensions
{
    /// <summary>
    /// Extract the actual inner update from <see cref="Update"/>
    /// </summary>
    /// <param name="update">The update.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">In case of Unknown update type.</exception>
    /// <exception cref="InvalidOperationException">In case of null inner update.</exception>
    public static object GetInnerUpdate(this Update update)
    {
        if (update.Type == UpdateType.Unknown)
            throw new ArgumentException(
                $"Can't resolve Update of Type {update.Type}", nameof(update));

        return typeof(Update).GetProperty(update.Type.ToString())?
            .GetValue(update, index: null) ?? throw new InvalidOperationException(
                $"Inner update is null for {update.Type}");
    }

    /// <summary>
    /// Extract the actual inner update from <see cref="Update"/>
    /// </summary>
    /// <param name="update">The update.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">In case of Unknown update type.</exception>
    /// <exception cref="InvalidOperationException">In case of null or invalid(type) inner update.</exception>
    public static T GetInnerUpdate<T>(this Update update)
    {
        if (update.Type == UpdateType.Unknown)
            throw new ArgumentException(
                $"Can't resolve Update of Type {update.Type}", nameof(update));

        return (T)(typeof(Update).GetProperty(update.Type.ToString(), typeof(T))?
            .GetValue(update, index: null) ?? throw new InvalidOperationException(
                $"Inner update is null for {update.Type}"));
    }

    private static bool TryResolveNamespaceToUpdateType(
        string currentNs,
        [NotNullWhen(true)] out UpdateType? updateType,
        [NotNullWhen(true)] out Type? type)
    {
        var nsParts = currentNs.Split('.');
        if (nsParts.Length < 3)
            throw new Exception("Namespace is invalid.");

        updateType = DictionaryNameToUpdateType(nsParts[^1]);

        if (updateType is null)
        {
            type = null;
            return false;
        }

        type = updateType.ToObjectType();

        if (type is null)
        {
            updateType = null;
            type = null;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Get the type of inner update that this update type referees to.
    /// </summary>
    /// <remarks>
    /// As instance, both <see cref="UpdateType.Message"/> and <see cref="UpdateType.EditedMessage"/> resolves to <see cref="Message"/>.
    /// </remarks>
    /// <param name="updateType"></param>
    /// <returns></returns>
    public static Type? ToObjectType(this UpdateType updateType)
    {
        return updateType switch
        {
            UpdateType.Message => typeof(Message),
            UpdateType.InlineQuery => typeof(InlineQuery),
            UpdateType.ChosenInlineResult => typeof(ChosenInlineResult),
            UpdateType.CallbackQuery => typeof(CallbackQuery),
            UpdateType.EditedMessage => typeof(Message),
            UpdateType.ChannelPost => typeof(Message),
            UpdateType.EditedChannelPost => typeof(Message),
            UpdateType.ShippingQuery => typeof(ShippingQuery),
            UpdateType.PreCheckoutQuery => typeof(PreCheckoutQuery),
            UpdateType.Poll => typeof(Poll),
            UpdateType.PollAnswer => typeof(PollAnswer),
            UpdateType.MyChatMember => typeof(ChatMemberUpdated),
            UpdateType.ChatMember => typeof(ChatMemberUpdated),
            UpdateType.ChatJoinRequest => typeof(ChatJoinRequest),

            // New updates
            UpdateType.MessageReaction => typeof(MessageReactionUpdated),
            UpdateType.MessageReactionCount => typeof(MessageReactionCountUpdated),
            UpdateType.ChatBoost => typeof(ChatBoostUpdated),
            UpdateType.RemovedChatBoost => typeof(ChatBoostRemoved),
            UpdateType.BusinessConnection => typeof(BusinessConnection),
            UpdateType.BusinessMessage => typeof(Message),
            UpdateType.EditedBusinessMessage => typeof(Message),
            UpdateType.DeletedBusinessMessages => typeof(BusinessMessagesDeleted),
            UpdateType.PurchasedPaidMedia => typeof(PaidMediaPurchased),

            // Default case
            _ => null,
        };
    }

    /// <summary>
    /// Get the type of inner update that this update type referees to.
    /// </summary>
    /// <remarks>
    /// As instance, both <see cref="UpdateType.Message"/> and <see cref="UpdateType.EditedMessage"/> resolves to <see cref="Message"/>.
    /// </remarks>
    /// <param name="updateType"></param>
    /// <returns></returns>
    public static Type? ToObjectType(this UpdateType? updateType)
        => updateType.HasValue ? updateType.Value.ToObjectType() : null;

    internal static UpdateType? DictionaryNameToUpdateType(this string name)
    {
        return name switch
        {
            "Messages" => UpdateType.Message,
            "EditedChannelPosts" => UpdateType.EditedChannelPost,
            "EditedMessages" => UpdateType.EditedMessage,
            "ChannelPosts" => UpdateType.ChannelPost,
            "CallbackQueries" => UpdateType.CallbackQuery,
            "InlineQueries" => UpdateType.InlineQuery,
            "ChosenInlineResults" => UpdateType.ChosenInlineResult,
            "ShippingQueries" => UpdateType.ShippingQuery,
            "PreCheckoutQueries" => UpdateType.PreCheckoutQuery,
            "Polls" => UpdateType.Poll,
            "PollAnswers" => UpdateType.PollAnswer,
            "ChatMembers" => UpdateType.ChatMember,
            "MyChatMembers" => UpdateType.MyChatMember,
            "ChatJoinRequests" => UpdateType.ChatJoinRequest,

            // New updates
            "MessageReactions" => UpdateType.MessageReaction,
            "MessageReactionCounts" => UpdateType.MessageReactionCount,
            "ChatBoosts" => UpdateType.ChatBoost,
            "RemovedChatBoosts" => UpdateType.RemovedChatBoost,
            "BusinessConnections" => UpdateType.BusinessConnection,
            "BusinessMessages" => UpdateType.BusinessMessage,
            "EditedBusinessMessages" => UpdateType.EditedBusinessMessage,
            "DeletedBusinessMessagess" => UpdateType.DeletedBusinessMessages,
            "PurchasedPaidMedia" => UpdateType.PurchasedPaidMedia,

            _ => null,
        };
    }

    /// <summary>
    /// Iter over all <see cref="IScopedUpdateHandler"/> with their type of update ( Eg: <see cref="Message"/>, ... )
    /// that can be collected.
    /// <para>You can add these to service collection to manually enable DI inside your scoped handlers.</para>
    /// </summary>
    /// <remarks>
    /// <b>Considerations:</b>
    /// <list type="number">
    /// <item>
    /// You should place handlers of different update types
    /// ( <see cref="UpdateType.Message"/>, <see cref="UpdateType.CallbackQuery"/>
    /// and etc. )
    /// into different parent folders.
    /// </item>
    /// <item>
    /// Parent name should match the update type name,
    /// Eg: <c>Messages</c> for <see cref="UpdateType.Message"/>
    /// </item>
    /// </list>
    /// Eg: <code><paramref name="handlersParentNamespace"/>/Messages/MyScopedMessageHandler</code>
    /// </remarks>
    /// <returns></returns>
    public static IEnumerable<(UpdateType updateType, Type update, Type handler)> IterCollectedScopedUpdateHandlerTypes(
        string handlersParentNamespace = "UpdateHandlers")
    {
        var entryAssembly = Assembly.GetEntryAssembly()
            ?? throw new InvalidOperationException("Can't find entry assembly.");
        var assemblyName = entryAssembly.GetName().Name;

        var handlerNs = $"{assemblyName}.{handlersParentNamespace}";

        // All types in *handlersParentNamespace*
        var scopedHandlersTypes = entryAssembly.GetTypes()
            .Where(x => x.Namespace is not null &&
                x.Namespace.StartsWith(handlerNs, StringComparison.Ordinal) && x.IsClass && typeof(IScopedUpdateHandler).IsAssignableFrom(x));

        foreach (var scopedType in scopedHandlersTypes)
        {
            if (!TryResolveNamespaceToUpdateType(
                scopedType.Namespace!, out var updateType1, out var type))
            {
                continue;
            }

            yield return (updateType1.Value, type, scopedType);
        }
    }

    /// <summary>
    /// Iter over all <see cref="IScopedUpdateHandlerContainer"/> that can be collected.
    /// </summary>
    /// <remarks>
    /// <b>Considerations:</b>
    /// <list type="number">
    /// <item>
    /// You should place handlers of different update types
    /// ( <see cref="UpdateType.Message"/>, <see cref="UpdateType.CallbackQuery"/>
    /// and etc. )
    /// into different parent folders.
    /// </item>
    /// <item>
    /// Parent name should match the update type name,
    /// eg: <c>Messages</c> for <see cref="UpdateType.Message"/>
    /// </item>
    /// </list>
    /// Eg: <paramref name="handlersParentNamespace"/>
    /// /Messages/MyScopedMessageHandler
    /// </remarks>
    /// <returns></returns>
    public static IEnumerable<HandlingInfo<IScopedUpdateHandlerContainer>> IterCollectedScopedContainers(
        string handlersParentNamespace = "UpdateHandlers")
    {
        foreach ((UpdateType updateType, Type update, Type handler) in
            IterCollectedScopedUpdateHandlerTypes(handlersParentNamespace))
        {
            var containerGeneric = typeof(ScopedUpdateHandlerContainerBuilder<,>)
                .MakeGenericType(handler, update);

            var container = (IScopedUpdateHandlerContainer?)Activator.CreateInstance(
                containerGeneric,
                [updateType, null, null]);

            if (container is null) continue;

            // This will be done in the updater main method
            //var extraInfo = handler.GetCustomAttribute<ScopedHandlerAttribute>();

            yield return new(container);
        }
    }

    /// <summary>
    /// Automatically collects all classes that are marked as scoped handlers
    /// And adds them to the <see cref="IUpdater"/> instance.
    /// </summary>
    /// <remarks>
    /// <b>Considerations:</b>
    /// <list type="number">
    /// <item>
    /// You should place handlers of different update types
    /// ( <see cref="UpdateType.Message"/>, <see cref="UpdateType.CallbackQuery"/>
    /// and etc. )
    /// into different parent folders.
    /// </item>
    /// <item>
    /// Parent name should match the update type name,
    /// Eg: <c>Messages</c> for <see cref="UpdateType.Message"/>
    /// </item>
    /// </list>
    /// Eg: <paramref name="handlersParentNamespace"/>
    /// /Messages/MyScopedMessageHandler
    /// </remarks>
    /// <returns></returns>
    public static IUpdater CollectScopedHandlers(
        this IUpdater updater,
        string handlersParentNamespace = "UpdateHandlers")
    {
        foreach (var container in IterCollectedScopedContainers(handlersParentNamespace))
        {
            if (container is null) continue;

            updater.Logger.LogInformation(
                "Scoped handler collected! ({Name})", container.Handler.ScopedHandlerType.Name);
            updater.AddScopedUpdateHandler(container.Handler);
        }

        return updater;
    }

    /// <summary>
    /// Use this to quickly add a message handler that responds /start command.
    /// </summary>
    /// <remarks>
    /// If you need a more advanced handler use <see cref="IUpdater.AddScopedUpdateHandler(IScopedUpdateHandlerContainer, HandlingOptions?)"/>
    /// or <see cref="IUpdater.AddSingletonUpdateHandler(ISingletonUpdateHandler, HandlingOptions?)"/>
    /// </remarks>
    /// <returns></returns>
    public static IUpdater QuickStartCommandReply(
        this IUpdater updater,
        string text,
        bool sendAsReply = true,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? messageEntities = default,
        bool? disableWebpagePreview = default,
        int? messageThreadId = default,
        bool disableNotification = default,
        ReplyMarkup? replyMarkup = default,
        bool protectContent = default,
        string? messageEffectId = default,
        string? businessConnectionId = default,
        bool allowPaidBroadcast = default,
        bool allowSendingWithoutReply = true,
        CancellationToken cancellationToken = default)
    {
        return updater.AddMessageHandler(
            container => container.Response(text: text,
                sendAsReply: sendAsReply,
                parseMode: parseMode,
                messageEntities: messageEntities,
                disableWebpagePreview: disableWebpagePreview,
                messageThreadId: messageThreadId,
                disableNotification: disableNotification,
                replyMarkup: replyMarkup,
                protectContent: protectContent,
                messageEffectId: messageEffectId,
                businessConnectionId: businessConnectionId,
                allowPaidBroadcast: allowPaidBroadcast,
                allowSendingWithoutReply: allowSendingWithoutReply,
                cancellationToken: cancellationToken),
            ReadyFilters.OnCommand("start") & ReadyFilters.PM());
    }

    /// <summary>
    /// Calls <see cref="TelegramBotClientExtensions.SetWebhook(ITelegramBotClient, string, InputFileStream?, string?, int?, IEnumerable{UpdateType}?, bool, string?, CancellationToken)"/>
    /// with provided arguments or via <see cref="UpdaterOptions"/> available inside <see cref="IUpdater.UpdaterOptions"/>.
    /// </summary>
    /// <returns></returns>
    public static Task SetWebhook(
        this IUpdater updater,
        string? url = default,
        InputFileStream? certificate = default,
        string? ipAddress = default,
        int? maxConnections = default,
        IEnumerable<UpdateType>? allowedUpdates = default,
        bool? dropPendingUpdates = default,
        CancellationToken cancellationToken = default)
    {
        return updater.BotClient.SetWebhook(
            url: (url ?? updater.UpdaterOptions.BotWebhookUrl?.AbsoluteUri)
                ?? throw new ArgumentNullException(nameof(url), "A url can't be fetched from neither parameters nor updater options."),
            certificate: certificate,
            ipAddress: ipAddress,
            maxConnections: maxConnections,
            allowedUpdates: allowedUpdates?? updater.UpdaterOptions.AllowedUpdates,
            dropPendingUpdates: dropPendingUpdates?? updater.UpdaterOptions.FlushUpdatesQueue,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Use this to start writing updates ( using a simple update writer )
    /// to the updater. ( Blocking )
    /// </summary>
    /// <param name="cancellationToken">To cancel the job manually,</param>
    /// <param name="updater">The updater.</param>
    public static async Task Start(
        this IUpdater updater, CancellationToken cancellationToken = default)
    {
        await updater.Start<DefaultUpdateWriter>(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Set commands from your filter using method
    /// <see cref="TelegramBotClientExtensions.SetMyCommands(ITelegramBotClient, IEnumerable{BotCommand}, BotCommandScope?, string?, CancellationToken)"/>.
    /// </summary>
    /// <param name="updater"></param>
    /// <returns></returns>
    public static async Task SetCommands(this IUpdater updater)
    {
        var singletonHandlerFilters = updater.SingletonUpdateHandlers
            .Where(x => x is IGenericSingletonUpdateHandler<Message>)
            .Cast<IGenericSingletonUpdateHandler<Message>>()
            .Where(x => x.Filter is not null)
            .Select(x => x.Filter!.DiscoverNestedFilters())
            .SelectMany(x => x)
            .Where(x => x is CommandFilter)
            .Cast<CommandFilter>();

        var scopedHandlerFilters = updater.ScopedHandlerContainers
            .Where(x => x is IGenericScopedUpdateHandlerContainer<Message>)
            .Cast<IGenericScopedUpdateHandlerContainer<Message>>()
            .Where(x => x.Filter is not null)
            .Select(x => x.Filter!.DiscoverNestedFilters())
            .SelectMany(x => x)
            .Where(x => x is CommandFilter)
            .Cast<CommandFilter>();

        var allCommands = singletonHandlerFilters.Concat(scopedHandlerFilters);

        var groupedCommands = allCommands.GroupBy(
            x => x.Options.BotCommandScope?.Type ?? BotCommandScopeType.Default);

        foreach (var scope in groupedCommands)
        {
            var commandScope = scope.First().Options.BotCommandScope;

            await updater.BotClient.SetMyCommands(scope
                    .Where(x => x.Options.Descriptions is not null)
                    .SelectMany(x => x.ToBotCommand())
                    .OrderBy(x => x.priority)
                    .Select(x => x.command),
                commandScope, cancellationToken: updater.UpdaterOptions.CancellationToken).ConfigureAwait(false);

            updater.Logger.LogInformation(
                "Set {count} commands to scope {scope}.",
                scope.Count(), commandScope?.Type ?? BotCommandScopeType.Default);
        }
    }

    /// <summary>
    /// Create a composite key with two values.
    /// </summary>
    /// <param name="First"></param>
    /// <param name="Second"></param>
    public readonly record struct CompositeKey(string First, string Second)
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $"({First},{Second})";
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <param name="key"></param>
        public static implicit operator string(CompositeKey key)
        {
            return key.ToString();
        }
    }

    internal static void SetCompositeItem<TValue>(
        this IUpdater updater,
        string outerKey,
        string key,
        TValue value,
        MemoryCacheEntryOptions? options = default)
    {
        updater.SetItem(new CompositeKey(outerKey, key), value, options);
    }

    internal static void SetCompositeItem<TValue>(
        this IUpdater updater,
        IChangeToken expirationToken,
        string outerKey,
        string key,
        TValue value)
    {
        var options = new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove,
        };

        options.AddExpirationToken(expirationToken);

        updater.SetCompositeItem(outerKey, key, value, options);
    }

    internal static void RemoveCompositeItem(
        this IUpdater updater, string outerKey, string key)
    {
        updater.RemoveItem(new CompositeKey(outerKey, key));
    }

    internal static void SetScopeItem<TValue>(
        this IUpdater updater,
        IChangeToken expirationToken,
        HandlingStoragesKeys.ScopeId scopeId,
        string key,
        TValue value)
    {
        updater.SetCompositeItem(expirationToken, scopeId.Id.ToString(), key, value);
    }

    internal static void SetLayerItem<TValue>(
        this IUpdater updater,
        IChangeToken expirationToken,
        HandlingStoragesKeys.LayerId layerId,
        string key,
        TValue value)
    {
        updater.SetCompositeItem(expirationToken, layerId.ToString(), key, value);
    }

    internal static void RemoveScopeItem(
        this IUpdater updater, HandlingStoragesKeys.ScopeId scopeId, string key)
    {
        updater.RemoveItem(new CompositeKey(scopeId.Id.ToString(), key));
    }

    internal static void RemoveLayerItem(
        this IUpdater updater, HandlingStoragesKeys.LayerId layerId, string key)
    {
        updater.RemoveItem(new CompositeKey(layerId.ToString(), key));
    }

    internal static bool TryGetCompositeItem<TValue>(
        this IUpdater updater,
        string outerKey,
        string key,
        [NotNullWhen(true)] out TValue? value)
        => updater.TryGetValue(new CompositeKey(outerKey, key), out value);

    internal static bool TryGetScopeItem<TValue>(
        this IUpdater updater, HandlingStoragesKeys.ScopeId scopeId, string key, [NotNullWhen(true)] out TValue? value)
        => updater.TryGetCompositeItem(scopeId.Id.ToString(), key, out value);

    internal static bool TryGetLayerItem<TValue>(
        this IUpdater updater, HandlingStoragesKeys.LayerId layerId, string key, [NotNullWhen(true)] out TValue? value)
        => updater.TryGetCompositeItem(layerId.ToString(), key, out value);

    #region Composite key
    /// <summary>
    /// Set a composite key item that expires when handling scope for this handler ends.
    /// </summary>
    public static void SetCompositeScopeItem<TValue>(
        this IUpdater updater,
        IChangeToken expirationToken,
        HandlingStoragesKeys.ScopeId scopeId,
        string firstKey,
        string secondKey,
        TValue value)
        => updater.SetScopeItem(expirationToken, scopeId, new CompositeKey(firstKey, secondKey), value);

    /// <summary>
    /// Remove a composite key item attached with this handler's scope id.
    /// </summary>
    public static void RemoveCompositeScopeItem(
        this IUpdater updater, HandlingStoragesKeys.ScopeId scopeId, string firstKey, string secondKey)
        => updater.RemoveScopeItem(scopeId, new CompositeKey(firstKey, secondKey));

    /// <summary>
    /// Get a composite key item that was set in handler's scope id.
    /// </summary>
    public static bool TryGetCompositeScopeItem<TValue>(
        this IUpdater updater,
        HandlingStoragesKeys.ScopeId scopeId,
        string firstKey,
        string secondKey,
        [NotNullWhen(true)] out TValue? value)
        => updater.TryGetScopeItem(scopeId, new CompositeKey(firstKey, secondKey), out value);

    /// <summary>
    /// Set a composite key item that expires when handling layer for this handler ends.
    /// </summary>
    public static void SetCompositeLayerItem<TValue>(
        this IUpdater updater,
        IChangeToken expirationToken,
        HandlingStoragesKeys.LayerId layerId,
        string firstKey,
        string secondKey,
        TValue value)
        => updater.SetLayerItem(expirationToken, layerId, new CompositeKey(firstKey, secondKey), value);

    /// <summary>
    /// Remove a composite key item attached with this handler's layer id.
    /// </summary>
    public static void RemoveCompositeLayerItem(
        this IUpdater updater, HandlingStoragesKeys.LayerId layerId, string firstKey, string secondKey)
        => updater.RemoveLayerItem(layerId, new CompositeKey(firstKey, secondKey));

    /// <summary>
    /// Get a composite key item that was set in handler's layer id.
    /// </summary>
    public static bool TryGetCompositeLayerItem<TValue>(
        this IUpdater updater,
        HandlingStoragesKeys.LayerId layerId,
        string firstKey,
        string secondKey,
        [NotNullWhen(true)] out TValue? value)
        => updater.TryGetLayerItem(layerId, new CompositeKey(firstKey, secondKey), out value);
    #endregion

    internal static HandlingOptions? GetHandlingOptionsFromAttibute(
        this IScopedUpdateHandlerContainer container)
    {
        return container.ScopedHandlerType
            .GetCustomAttribute<ScopedHandlerAttribute>()?
            .GetHandlingOptions();
    }

    internal static HandlingOptions? GetHandlingOptionsFromAttibute(
        this ISingletonUpdateHandler container)
    {
        return container
            .GetType()
            .GetCustomAttribute<SingletonHandlerCallbackAttribute>()?
            .GetHandlingOptions();
    }

    internal static UpdaterOptions RedesignOptions(
        UpdaterOptions? updaterOptions = null,
        IServiceProvider? serviceProvider = default,
        string? newBotToken = default)
    {
        var fetchedOption = updaterOptions ?? (serviceProvider?.GetService<IConfiguration>()?
            .GetSection(UpdaterOptions.Updater).Get<UpdaterOptions>());

        var logger = fetchedOption?.Logger ?? serviceProvider?.GetService<ILogger<IUpdater>>();

        if (updaterOptions is null)
            logger?.LogWarning("No updater option passed or can be found from configuration.");

        return new UpdaterOptions(
            botToken: newBotToken ?? fetchedOption?.BotToken,
            maxDegreeOfParallelism: fetchedOption?.MaxDegreeOfParallelism,
            logger: logger,
            cancellationToken: fetchedOption?.CancellationToken ?? default,
            flushUpdatesQueue: fetchedOption?.FlushUpdatesQueue ?? default,
            allowedUpdates: fetchedOption?.AllowedUpdates
        )
        {
            BotWebhookUrl = fetchedOption?.BotWebhookUrl,
            SecretToken = fetchedOption?.SecretToken,
        };
    }
}