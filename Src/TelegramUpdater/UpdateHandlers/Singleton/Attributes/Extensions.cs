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
    internal static Type? GetContainerType(this Type containerType)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(containerType);
#else
        if (containerType == null)
            throw new ArgumentNullException(nameof(containerType));
#endif

        if (containerType.IsInterface && containerType.IsGenericType)
        {
            var genericDef = containerType.GetGenericTypeDefinition();
            if (genericDef == typeof(IContainer<>))
            {
                return containerType.GetGenericArguments()[0];
            }
        }

        return null;
    }

    internal static ISingletonUpdateHandler? CreateHandlerOfType<T>(
        UpdateType updateType,
        Func<Update, T?> resolver,
        MethodInfo method) where T : class
    {
        var filters = method.GetFilterAttributes<UpdaterFilterInputs<T>>();

        try
        {
            var callback = (Func<IContainer<T>, Task>)Delegate
                .CreateDelegate(typeof(Func<IContainer<T>, Task>), method);

            return new DefaultHandler<T>(updateType, getT: resolver, callback: callback, filter: filters);
        }
        catch (ArgumentException)
        {
            //var message = $"Expected: IContainer<{typeof(T)}>, but found {method.GetParameters()[0].ParameterType}.";
            // TODO: Log the message or handle it as needed
            // Console.WriteLine(message);
            return null;
        }
    }

    internal static ISingletonUpdateHandler? GetSingletonUpdateHandler(
        MethodInfo method, UpdateType updateType)
    {
        var propertyInfo = typeof(Update).GetProperty(updateType.ToString());
        if (propertyInfo == null)
            return null;

        var resolverType = typeof(Func<,>).MakeGenericType(typeof(Update), propertyInfo.PropertyType);
        var resolver = Delegate.CreateDelegate(resolverType, propertyInfo.GetGetMethod()!);

        var createHandlerMethod = typeof(SingletonAttributesExtensions)
            .GetMethod(nameof(CreateHandlerOfType), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(propertyInfo.PropertyType!);

        return (ISingletonUpdateHandler?)createHandlerMethod.Invoke(null, [updateType, resolver, method]);
    }

    /// <summary>
    /// Use this method to iter over all methods that are marked with
    /// <see cref="HandlerCallbackAttribute"/>.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public static IEnumerable<HandlingInfo<ISingletonUpdateHandler>> IterSingletonUpdateHandlerCallbacks()
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
            var singletonAttr = method.GetCustomAttribute<HandlerCallbackAttribute>();

            if (singletonAttr is null) continue;

            var parameters = method.GetParameters();
            if (parameters.Length == 1)
            {
                var containerType = parameters[0].ParameterType.GetContainerType();
                if (containerType is not null)
                {
                    // TODO: can we figure out update type from type of update only? No. A message can be Message, EditedMessage and ...
                    //if (!Enum.TryParse(containerType.Name, out UpdateType ut)) continue;

                    //if (ut != singletonAttr.UpdateType) continue;

                    var handler = GetSingletonUpdateHandler(method, singletonAttr.UpdateType);

                    if (handler is not null)
                    {
                        yield return new(handler, singletonAttr.GetHandlingOptions());
                    }
                }
            }
        }
    }

    /// <summary>
    /// Use this method to collect all methods that are marked with
    /// <see cref="HandlerCallbackAttribute"/>. And add them
    /// to the <paramref name="updater"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public static IUpdater CollectHandlingCallbacks(
        this IUpdater updater)
    {
        foreach (var handler in IterSingletonUpdateHandlerCallbacks())
        {
            updater.AddSingletonUpdateHandler(handler.Handler, handler.Options);
        }

        return updater;
    }
}
