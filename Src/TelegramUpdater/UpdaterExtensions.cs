// Ignore Spelling: Iter

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Telegram.Bot.Types.Payments;
using TelegramUpdater.Filters;
using TelegramUpdater.UpdateHandlers.Scoped;
using TelegramUpdater.UpdateHandlers.Scoped.Attributes;
using TelegramUpdater.UpdateHandlers.Singleton;
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
            .GetValue(update, index: null)?? throw new InvalidOperationException(
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
            .GetValue(update, index: null)?? throw new InvalidOperationException(
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

            var extrainfo = handler.GetCustomAttribute<ScopedHandlerAttribute>();

            yield return new(container, extrainfo?.Group?? default);
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
    public static IUpdater AutoCollectScopedHandlers(
        this IUpdater updater,
        string handlersParentNamespace = "UpdateHandlers")
    {
        foreach (var container in IterCollectedScopedContainers(handlersParentNamespace))
        {
            if (container is null) continue;

            updater.Logger.LogInformation(
                "Scoped handler collected! ( {Name} )", container.Handler.ScopedHandlerType.Name);
            updater.AddScopedUpdateHandler(container.Handler, container.Group);
        }

        return updater;
    }

    /// <summary>
    /// Use this to start writing updates ( using a simple update writer )
    /// to the updater. ( Blocking )
    /// </summary>
    /// <param name="cancellationToken">To cancel the job manually,</param>
    /// <param name="updater">The updater.</param>
    public static async Task StartAsync(
        this IUpdater updater, CancellationToken cancellationToken = default)
    {
        await updater.StartAsync<SimpleUpdateWriter>(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Set commands from your filter using method
    /// <see cref="TelegramBotClientExtensions.SetMyCommands(ITelegramBotClient, IEnumerable{BotCommand}, BotCommandScope?, string?, CancellationToken)"/>.
    /// </summary>
    /// <param name="updater"></param>
    /// <returns></returns>
    public static async Task SetCommandsAsync(this IUpdater updater)
    {
        var singletonHandlerFilters = updater.SingletonUpdateHandlers
            .Where(x => x is IGenericSingletonUpdateHandler<Message>)
            .Cast<IGenericSingletonUpdateHandler<Message>>()
            .Where(x => x.Filter is not null)
            .Select(x => x.Filter!.DiscoverNestedFilters())
            .SelectMany(x => x)
            .Where(x=> x is CommandFilter)
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
            x => x.Options.BotCommandScope?.Type?? BotCommandScopeType.Default);

        foreach (var scope in groupedCommands)
        {
            var commandScope = scope.First().Options.BotCommandScope;

            await updater.BotClient.SetMyCommands(scope
                    .Where(x=> x.Options.Descriptions is not null)
                    .SelectMany(x => x.ToBotCommand())
                    .OrderBy(x=> x.priority)
                    .Select(x=> x.command),
                commandScope, cancellationToken: updater.UpdaterOptions.CancellationToken).ConfigureAwait(false);

            updater.Logger.LogInformation(
                "Set {count} commands to scope {scope}.",
                scope.Count(), commandScope?.Type?? BotCommandScopeType.Default);
        }
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
            botToken: newBotToken?? fetchedOption?.BotToken,
            maxDegreeOfParallelism: fetchedOption?.MaxDegreeOfParallelism,
            logger: logger,
            cancellationToken: fetchedOption?.CancellationToken?? default,
            flushUpdatesQueue: fetchedOption?.FlushUpdatesQueue?? default,
            allowedUpdates: fetchedOption?.AllowedUpdates
        );
    }
}
