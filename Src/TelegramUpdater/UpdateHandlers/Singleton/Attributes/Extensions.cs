using System.Reflection;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Singleton;
using TelegramUpdater.UpdateHandlers.Singleton.Attributes;
using TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

namespace TelegramUpdater;

/// <summary>
/// Extensions for Singleton update handler attributes.
/// </summary>
public static class SingletonAttributesExtensions
{
    internal static Type? GetContainerType(this Type container)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(container);
#else
        if (container == null)
            throw new ArgumentNullException(nameof(container));
#endif

        if (container.IsInterface && container.IsGenericType)
        {
            var genericDef = container.GetGenericTypeDefinition();
            if (genericDef == typeof(IContainer<>))
            {
                return container.GetGenericArguments()[0];
            }
        }

        return null;
    }

    internal static ISingletonUpdateHandler? CreateHandlerOfType<T>(
        UpdateType updateType,
        Func<Update, T?> resolver,
        MethodInfo method,
        int group) where T : class
    {
        var filters = method.GetFilterAttributes<T>();
        var callback = (Func<IContainer<T>, Task>)Delegate
            .CreateDelegate(typeof(Func<IContainer<T>, Task>), method);

        return new AnyHandler<T>(updateType, resolver, callback, filters, group);
    }

    internal static ISingletonUpdateHandler? GetSingletonUpdateHandler(
        MethodInfo method, UpdateType updateType, int group)
    {
        return updateType switch
        {
            UpdateType.Message
                => CreateHandlerOfType(updateType, x => x.Message, method, group),
            UpdateType.InlineQuery
                => CreateHandlerOfType(updateType, x => x.InlineQuery, method, group),
            UpdateType.ChosenInlineResult
                => CreateHandlerOfType(updateType, x => x.ChosenInlineResult, method, group),
            UpdateType.CallbackQuery
                => CreateHandlerOfType(updateType, x => x.CallbackQuery, method, group),
            UpdateType.EditedMessage
                => CreateHandlerOfType(updateType, x => x.EditedMessage, method, group),
            UpdateType.ChannelPost
                => CreateHandlerOfType(updateType, x => x.ChannelPost, method, group),
            UpdateType.EditedChannelPost
                => CreateHandlerOfType(updateType, x => x.EditedChannelPost, method, group),
            UpdateType.ShippingQuery
                => CreateHandlerOfType(updateType, x => x.ShippingQuery, method, group),
            UpdateType.PreCheckoutQuery
                => CreateHandlerOfType(updateType, x => x.PreCheckoutQuery, method, group),
            UpdateType.Poll
                => CreateHandlerOfType(updateType, x => x.Poll, method, group),
            UpdateType.PollAnswer
                => CreateHandlerOfType(updateType, x => x.PollAnswer, method, group),
            UpdateType.MyChatMember
                => CreateHandlerOfType(updateType, x => x.MyChatMember, method, group),
            UpdateType.ChatMember
                => CreateHandlerOfType(updateType, x => x.ChatMember, method, group),
            UpdateType.ChatJoinRequest
                => CreateHandlerOfType(updateType, x => x.ChatJoinRequest, method, group),

            // TODO: add new updates

            _ => null,
        };
    }

    /// <summary>
    /// Use this method to iter over all methods that are marked with
    /// <see cref="SingletonHandlerCallbackAttribute"/>.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public static IEnumerable<ISingletonUpdateHandler> IterSingletonUpdateHandlerCallbacks()
    {
        var entryAssembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Can't find entry assembly.");
        var assemplyName = entryAssembly.GetName().Name;

        var methods = entryAssembly.GetTypes()
            .Where(x => x.IsClass)
            .SelectMany(x => x.GetMethods(
                BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod))
            .Where(x => x.ReturnType == typeof(Task));

        foreach (var method in methods)
        {
            var singletonAttr = method.GetCustomAttribute<SingletonHandlerCallbackAttribute>();

            if (singletonAttr is null) continue;

            var parameters = method.GetParameters();
            if (parameters.Length == 1)
            {
                var containerType = parameters[0].ParameterType.GetContainerType();
                if (containerType is not null)
                {
                    if (!Enum.TryParse(containerType.Name, out UpdateType ut)) continue;

                    if (ut != singletonAttr.UpdateType) continue;

                    var handler = GetSingletonUpdateHandler(
                        method, singletonAttr.UpdateType, singletonAttr.Group);

                    if (handler is not null)
                    {
                        yield return handler;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Use this method to collect all methods that are marked with
    /// <see cref="SingletonHandlerCallbackAttribute"/>. And add them
    /// to the <paramref name="updater"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public static IUpdater CollectSingletonUpdateHandlerCallbacks(
        this IUpdater updater)
    {
        foreach (var handler in IterSingletonUpdateHandlerCallbacks())
        {
            updater.AddSingletonUpdateHandler(handler);
        }

        return updater;
    }
}
