using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Telegram.Bot.Types.Payments;
using TelegramUpdater.Filters;
using TelegramUpdater.UpdateHandlers.Scoped;
using TelegramUpdater.UpdateHandlers.Singleton;
using TelegramUpdater.UpdateWriters;

namespace TelegramUpdater;

/// <summary>
/// Extension methods for <see cref="IUpdater"/>.
/// </summary>
public static class UpdaterExtensions
{
    internal static object GetInnerUpdate(this Update update)
    {
        if (update.Type == UpdateType.Unknown)
            throw new ArgumentException(
                $"Can't resolve Update of Type {update.Type}");

        return typeof(Update).GetProperty(update.Type.ToString())?
            .GetValue(update, null)?? throw new InvalidOperationException(
                $"Inner update is null for {update.Type}");
    }

    internal static T GetInnerUpdate<T>(this Update update)
    {
        if (update.Type == UpdateType.Unknown)
            throw new ArgumentException(
                $"Can't resolve Update of Type {update.Type}");

        return (T)(typeof(Update).GetProperty(update.Type.ToString())?
            .GetValue(update, null)?? throw new InvalidOperationException(
                $"Inner update is null for {update.Type}"));
    }

    internal static UpdateType? GetUpdateType<T>()
    {
        if (Enum.TryParse(typeof(T).Name, out UpdateType result))
        {
            return result;
        }

        return null;
    }

    private static bool TryResolveNamespaceToUpdateType(
        string currentNs,
        [NotNullWhen(true)] out UpdateType? updateType,
        [NotNullWhen(true)] out Type? type)
    {
        var nsParts = currentNs.Split('.');
        if (nsParts.Length < 3)
            throw new Exception("Namespace is invalid.");

        updateType = nsParts[2] switch
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
            _ => null
        };

        if (updateType is null)
        {
            type = null;
            return false;
        }

        type = updateType switch
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
            _ => null
        };

        if (type is null)
        {
            updateType = null;
            type = null;
            return false;
        }

        return true;
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
    /// eg: <c>Messages</c> for <see cref="UpdateType.Message"/>
    /// </item>
    /// </list>
    /// Eg: <paramref name="handlersParentNamespace"/>
    /// /Messages/MyScopedMessageHandler
    /// </remarks>
    /// <returns></returns>
    public static IEnumerable<(UpdateType updateType, Type update, Type handler)> IterCollectedScopedUpdateHandlerTypes(
        string handlersParentNamespace = "UpdateHandlers")
    {
        var entryAssembly = Assembly.GetEntryAssembly();

        if (entryAssembly is null)
            throw new ApplicationException("Can't find entry assembly.");

        var assemblyName = entryAssembly.GetName().Name;

        var handlerNs = $"{assemblyName}.{handlersParentNamespace}";

        // All types in *handlersParentNamespace*
        var scopedHandlersTypes = entryAssembly.GetTypes()
            .Where(x =>
                x.Namespace is not null &&
                x.Namespace.StartsWith(handlerNs))
            .Where(x => x.IsClass)
            .Where(x => typeof(IScopedUpdateHandler).IsAssignableFrom(x));

        foreach (var scopedType in scopedHandlersTypes)
        {
            if (!TryResolveNamespaceToUpdateType(
                scopedType.Namespace!, out var updateType1, out var type))
            {
                continue;
            };

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
    public static IEnumerable<IScopedUpdateHandlerContainer> IterCollectedScopedContainers(
        string handlersParentNamespace = "UpdateHandlers")
    {
        foreach ((UpdateType updateType, Type update, Type handler) in
            IterCollectedScopedUpdateHandlerTypes(handlersParentNamespace))
        {
            var containerGeneric = typeof(ScopedUpdateHandlerContainerBuilder<,>)
                .MakeGenericType(handler, update);

            var container = (IScopedUpdateHandlerContainer?)Activator.CreateInstance(
                containerGeneric,
                new object?[] { updateType, null, null });

            if (container is null) continue;

            yield return container;
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
    /// eg: <c>Messages</c> for <see cref="UpdateType.Message"/>
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
                "Scoped handler collected! ( {Name} )", container.ScopedHandlerType.Name);
            updater.AddScopedUpdateHandler(container);
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
        await updater.StartAsync<SimpleUpdateWriter>(cancellationToken);
    }

    /// <summary>
    /// Set commands from your filter using method
    /// <see cref="TelegramBotClientExtensions.SetMyCommandsAsync(
    /// ITelegramBotClient, IEnumerable{BotCommand},
    /// BotCommandScope?, string?, CancellationToken)"/>.
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

            await updater.BotClient.SetMyCommandsAsync(
                scope.SelectMany(x => x.ToBotCommand())
                    .OrderBy(x=> x.priority)
                    .Select(x=> x.command),
                commandScope);

            updater.Logger.LogInformation(
                "Set {count} commands to scope {scope}.",
                scope.Count(), commandScope?.Type?? BotCommandScopeType.Default);
        }
    }
}
