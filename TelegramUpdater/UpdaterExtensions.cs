using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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
            throw new ArgumentException($"Can't resolve Update of Type {update.Type}");

        return typeof(Update).GetProperty(update.Type.ToString())?.GetValue(update, null)
            ?? throw new InvalidOperationException($"Inner update is null for {update.Type}");
    }

    internal static T GetInnerUpdate<T>(this Update update)
    {
        if (update.Type == UpdateType.Unknown)
            throw new ArgumentException($"Can't resolve Update of Type {update.Type}");

        return (T)(typeof(Update).GetProperty(update.Type.ToString())?.GetValue(update, null)
            ?? throw new InvalidOperationException($"Inner update is null for {update.Type}"));
    }

    internal static UpdateType? GetUpdateType<T>()
    {
        if (Enum.TryParse(typeof(T).Name, out UpdateType result))
        {
            return result;
        }

        return null;
    }

    private static bool TryResovleNamespaceToUpdateType(
        string currentNs, [NotNullWhen(true)] out Type? type)
    {
        var nsParts = currentNs.Split('.');
        if (nsParts.Length < 3)
            throw new Exception("Namespace is invalid.");

        type = nsParts[2] switch
        {
            "Messages" => typeof(Message),
            "CallbackQueries" => typeof(CallbackQuery),
            "InlineQueries" => typeof(InlineQuery),
            _ => null
        };

        if (type is null)
            return false;
        return true;
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
    /// ( <see cref="UpdateType.Message"/>, <see cref="UpdateType.CallbackQuery"/> and etc. )
    /// into different parent folders.
    /// </item>
    /// <item>
    /// Parent name should match the update type name, eg: <c>Messages</c> for <see cref="UpdateType.Message"/>
    /// </item>
    /// </list>
    /// Eg: <paramref name="handlersParentNamespace"/>/Messages/MyScopedMessageHandler
    /// </remarks>
    /// <returns></returns>
    public static IUpdater AutoCollectScopedHandlers(
        this IUpdater updater,
        string handlersParentNamespace = "UpdateHandlers")
    {
        var entryAssembly = Assembly.GetEntryAssembly();

        if (entryAssembly is null)
            throw new ApplicationException("Can't find entry assembly.");

        var assemplyName = entryAssembly.GetName().Name;

        var handlerNs = $"{assemplyName}.{handlersParentNamespace}";

        // All types in *handlersParentNamespace*
        var scopedHandlersTypes = entryAssembly.GetTypes()
            .Where(x =>
                x.Namespace is not null &&
                x.Namespace.StartsWith(handlerNs))
            .Where(x => x.IsClass)
            .Where(x => typeof(IScopedUpdateHandler).IsAssignableFrom(x));

        foreach (var scopedType in scopedHandlersTypes)
        {
            if (!TryResovleNamespaceToUpdateType(scopedType.Namespace!, out var updateType))
            {
                continue;
            }

            var containerGeneric = typeof(ScopedUpdateHandlerContainerBuilder<,>)
                .MakeGenericType(scopedType, updateType);

            var container = (IScopedUpdateHandlerContainer?)Activator.CreateInstance(
                containerGeneric,
                new object?[]
                {
                        Enum.Parse<UpdateType>(updateType.Name), null, null
                });

            if (container is null) continue;

            updater.Logger.LogInformation("Scoped handler collected! ( {Name} )", scopedType.Name);
            updater.AddScopedUpdateHandler(container);
        }

        return updater;
    }

    /// <summary>
    /// Use this to start writing updates ( using a simple update writer ) to the updater. ( Blocking )
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
    /// <see cref="TelegramBotClientExtensions.SetMyCommandsAsync(ITelegramBotClient, IEnumerable{BotCommand}, BotCommandScope?, string?, CancellationToken)"/>.
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

        var groupedCommands = allCommands.GroupBy(x => x.Options.BotCommandScope);

        foreach (var scope in groupedCommands)
        {
            await updater.BotClient.SetMyCommandsAsync(
                scope.SelectMany(x => x.ToBotCommand()), scope.Key);

            updater.Logger.LogInformation(
                "Set {count} commands to scope {scope}.",
                scope.Count(), scope.Key?.Type?? BotCommandScopeType.Default);
        }
    }
}
