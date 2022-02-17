using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.SealedHandlers;

namespace TelegramUpdater;

public static class UpdateHandlerExtensions
{
    /// <summary>
    /// Add any update handler to the updater.
    /// </summary>
    /// <typeparam name="T">Your update type, eg: <see cref="Message"/></typeparam>.
    /// <param name="updater"></param>
    /// <param name="updateSelector">
    /// A function to select the right update from <see cref="Update"/>
    /// <para>Eg: <code>update => update.Message</code></para>
    /// </param>
    /// <param name="callback">Callback function to handle your update.</param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">Handling priority.</param>
    public static IUpdater AddUpdateHandler<T>(
        this IUpdater updater,
        Func<Update, T?> updateSelector,
        Func<IContainer<T>, Task> callback,
        Filter<T>? filter = default,
        int group = 0)
        where T : class
    {
        var t = typeof(T);

        if (!Enum.TryParse(t.Name, out UpdateType updateType))
        {
            throw new InvalidCastException($"{t} is not an Update.");
        }

        return updater.AddUpdateHandler(
            new AnyUpdateHandler<T>(updateType, updateSelector, callback, filter, group));
    }
}
