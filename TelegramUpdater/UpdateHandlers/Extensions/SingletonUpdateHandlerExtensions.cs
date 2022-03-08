using Telegram.Bot.Types.Payments;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Singleton.ReadyToUse;

namespace TelegramUpdater;

/// <summary>
/// A set of extension methods for <see cref="IUpdater"/> handlers.
/// </summary>
public static class SingletonUpdateHandlerExtensions
{
    /// <summary>
    /// Adds any singleton update handler to the updater.
    /// </summary>
    /// <typeparam name="T">Your update type, eg: <see cref="Message"/></typeparam>.
    /// <param name="updater"></param>
    /// <param name="updateSelector">
    /// A function to select the right update from <see cref="Update"/>
    /// <para>Eg: <code>update => update.Message</code></para>
    /// </param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
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
    /// Adds a handler for any update that is a <see cref="Message"/>.
    /// </summary>
    /// <param name="updateType">
    /// Type of update. should be one of <see cref="UpdateType.Message"/>,
    /// <see cref="UpdateType.EditedMessage"/>, <see cref="UpdateType.ChannelPost"/>,
    /// <see cref="UpdateType.EditedChannelPost"/>.
    /// </param>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        UpdateType updateType,
        Func<IContainer<Message>, Task> callback,
        Filter<Message>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            updateType switch
            {
                UpdateType.Message
                    => new MessageHandler(callback, filter, group),
                UpdateType.EditedMessage
                    => new EditedMessageHandler(callback, filter, group),
                UpdateType.ChannelPost
                    => new ChannelPostHandler(callback, filter, group),
                UpdateType.EditedChannelPost
                    => new EditedChannelPostHandler(callback, filter, group),
                _ => throw new ArgumentException(
                    $"Update type {updateType} is not a Message."
                )
            });

    /// <summary>
    /// Adds a handler for any update that is a <see cref="ChatMemberUpdated"/>.
    /// </summary>
    /// <param name="updateType">
    /// Type of update. should be one of
    /// <see cref="UpdateType.ChatMember"/>, <see cref="UpdateType.MyChatMember"/>.
    /// </param>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        UpdateType updateType,
        Func<IContainer<ChatMemberUpdated>, Task> callback,
        Filter<ChatMemberUpdated>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            updateType switch
            {
                UpdateType.ChatMember
                    => new ChatMemberHandler(callback, filter, group),
                UpdateType.MyChatMember
                    => new MyChatMemberHandler(callback, filter, group),
                _ => throw new ArgumentException(
                    $"Update type {updateType} is not a ChatMemberUpdated."
                )
            });

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.CallbackQuery"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<CallbackQuery>, Task> callback,
        Filter<CallbackQuery>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new CallbackQueryHandler(callback, filter, group));

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.InlineQuery"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<InlineQuery>, Task> callback,
        Filter<InlineQuery>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new InlineQueryHandler(callback, filter, group));

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.ChosenInlineResult"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<ChosenInlineResult>, Task> callback,
        Filter<ChosenInlineResult>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new ChosenInlineResultHandler(callback, filter, group));

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.ChatJoinRequest"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<ChatJoinRequest>, Task> callback,
        Filter<ChatJoinRequest>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new ChatJoinRequestHandler(callback, filter, group));

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.Poll"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<Poll>, Task> callback,
        Filter<Poll>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new PollHandler(callback, filter, group));

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.PollAnswer"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<PollAnswer>, Task> callback,
        Filter<PollAnswer>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new PollAnswerHandler(callback, filter, group));

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.PreCheckoutQuery"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<PreCheckoutQuery>, Task> callback,
        Filter<PreCheckoutQuery>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new PreCheckoutQueryHandler(callback, filter, group));

    /// <summary>
    /// Adds a handler for <see cref="UpdateType.ShippingQuery"/>.
    /// </summary>
    /// <param name="updater">The updater.</param>
    /// <param name="callback">
    /// Callback function to handle your update.
    /// </param>
    /// <param name="filter">Filters.</param>
    /// <param name="group">
    /// Handling priority group, The lower the sooner to process.
    /// </param>
    public static IUpdater AddSingletonUpdateHandler(
        this IUpdater updater,
        Func<IContainer<ShippingQuery>, Task> callback,
        Filter<ShippingQuery>? filter = default,
        int group = default)
        => updater.AddSingletonUpdateHandler(
            new ShippingQueryHandler(callback, filter, group));
}
