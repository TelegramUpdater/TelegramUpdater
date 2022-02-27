using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TelegramUpdater.UpdateHandlers.ScopedHandlers;

namespace TelegramUpdater
{
    public static class UpdaterExtensions
    {
        public static object GetInnerUpdate(this Update update)
        {
            if (update.Type == UpdateType.Unknown)
                throw new ArgumentException($"Can't resolve Update of Type {update.Type}");

            return typeof(Update).GetProperty(update.Type.ToString())?.GetValue(update, null)
                ?? throw new InvalidOperationException($"Inner update is null for {update.Type}");
        }

        public static T GetInnerUpdate<T>(this Update update)
        {
            if (update.Type == UpdateType.Unknown)
                throw new ArgumentException($"Can't resolve Update of Type {update.Type}");

            return (T)(typeof(Update).GetProperty(update.Type.ToString())?.GetValue(update, null)
                ?? throw new InvalidOperationException($"Inner update is null for {update.Type}"));
        }

        public static UpdateType? GetUpdateType<T>()
        {
            if (Enum.TryParse(typeof(T).Name, out UpdateType result))
            {
                return result;
            }

            return null;
        }

        private static bool TryResovleNamespaceToUpdateType(
            string handlersParentNamespace, string currentNs, [NotNullWhen(true)]out Type? type)
        {
            var nsParts = currentNs.Split('.');
            if (nsParts.Length != 3)
                throw new Exception("Namespace is invalid.");

            if ($"{nsParts[0]}.{nsParts[1]}" != handlersParentNamespace)
                throw new Exception("Base namespace not matching the handler namespace");

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

            var allTypes = entryAssembly.GetTypes();
            var assemplyName = entryAssembly.GetName().Name;

            var handlerNs = $"{assemplyName}.{handlersParentNamespace}";

            // All types in *handlersParentNamespace*
            var typesInNamespace = allTypes
                .Where(x =>
                    x.Namespace is not null &&
                    x.Namespace.StartsWith(handlerNs));

            var validTypesInNamespace = typesInNamespace
                .Where(x => x.IsClass);

            var scopedHandlersTypes = validTypesInNamespace
                .Where(x=> typeof(IScopedUpdateHandler).IsAssignableFrom(x));

            foreach (var scopedType in scopedHandlersTypes)
            {
                if (!TryResovleNamespaceToUpdateType(
                    handlerNs, scopedType.Namespace!, out var updateType))
                {
                    continue;
                }

                var containerGeneric = typeof(UpdateContainerBuilder<,>)
                    .MakeGenericType(scopedType, updateType);

                var container = (IScopedHandlerContainer?)Activator.CreateInstance(
                    containerGeneric,
                    new object?[]
                    {
                        Enum.Parse<UpdateType>(updateType.Name), null, null
                    });

                if (container is null) continue;

                updater.Logger.LogInformation("Scoped handler collected! ( {Name} )", scopedType.Name);
                return updater.AddScopedHandler(container);
            }

            return updater;
        }
    }
}
