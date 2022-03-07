using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

namespace TelegramUpdater;

/// <summary>
/// A set of extension methods for <see cref="IUpdater"/> handlers.
/// </summary>
public static class SingletonUpdateHandlerExtensions
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
    public static IUpdater AddSingletonUpdateHandler<T>(
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

        return updater.AddSingletonUpdateHandler(
            new AnyHandler<T>(updateType, updateSelector, callback, filter, group));
    }

    /// <summary>
    /// Add <see cref="Message"/> handler to the updater.
    /// </summary>
    /// <param name="updater"></param>
    /// <param name="callback">Callback function to handle your update.</param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">Handling priority.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<Message>, Task> callback,
        Filter<Message>? filter = default,
        int group = default)
    {
        return updater.AddSingletonUpdateHandler(new MessageHandler(callback, filter, group));
    }

    /// <summary>
    /// Add <see cref="CallbackQuery"/> handler to the updater.
    /// </summary>
    /// <param name="updater"></param>
    /// <param name="callback">Callback function to handle your update.</param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">Handling priority.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<CallbackQuery>, Task> callback,
        Filter<CallbackQuery>? filter = default,
        int group = default)
    {
        return updater.AddSingletonUpdateHandler(
            new CallbackQueryHandler(callback, filter, group));
    }

    /// <summary>
    /// Add <see cref="InlineQuery"/> handler to the updater.
    /// </summary>
    /// <param name="updater"></param>
    /// <param name="callback">Callback function to handle your update.</param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">Handling priority.</param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<InlineQuery>, Task> callback,
        Filter<InlineQuery>? filter = default,
        int group = default)
    {
        return updater.AddSingletonUpdateHandler(
            new InlineQueryHandler(callback, filter, group));
    }
}
