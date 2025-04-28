using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Payments;
using TelegramUpdater.UpdateHandlers.Scoped;

namespace TelegramUpdater;

/// <summary>
/// A set of useful extension methods for <see cref="IScopedUpdateHandler"/>s.
/// </summary>
public static class ScopedUpdateHandlersExtensions
{
    /// <summary>
    /// Adds an scoped handler to the updater.
    /// </summary>
    /// <typeparam name="TUpdate">Update type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="typeOfScopedHandler">Type of your handler.</param>
    /// <param name="updateType">Update type again.</param>
    /// <param name="filter">A filter to choose the right update.</param>
    /// <param name="getT">
    /// A function to choose real update from <see cref="Update"/>
    /// <para>Don't touch it if you don't know.</para>
    /// </param>
    /// <param name="group"></param>
    /// <remarks>This method will add filter attributes if <paramref name="filter"/> is null.</remarks>
    public static IUpdater AddScopedUpdateHandler<TUpdate>(
        this IUpdater updater,
        Type typeOfScopedHandler,
        UpdaterFilter<TUpdate>? filter = default,
        UpdateType? updateType = default,
        Func<Update, TUpdate>? getT = default,
        int group = default) where TUpdate : class
    {
        if (!typeof(IScopedUpdateHandler).IsAssignableFrom(typeOfScopedHandler))
        {
            throw new InvalidCastException(
                $"{typeOfScopedHandler} Should be an IScopedUpdateHandler");
        }

        var _t = typeof(TUpdate);
        if (updateType == null)
        {
            if (Enum.TryParse(_t.Name, out UpdateType ut))
            {
                updateType = ut;
            }
            else
            {
                throw new InvalidCastException($"{_t} is not an Update! Should be Message, CallbackQuery, ...");
            }
        }

        var containerGeneric = typeof(ScopedUpdateHandlerContainerBuilder<,>).MakeGenericType(
            typeOfScopedHandler, _t);

        var container = (IScopedUpdateHandlerContainer?)Activator.CreateInstance(
            containerGeneric, [updateType.Value, filter, getT]);

        if (container != null)
        {
            return updater.AddScopedUpdateHandler(container, group);
        }

        updater.Logger.LogWarning(
            "{type} not added to the Scoped Handlers! The instance of it is null.",
            typeOfScopedHandler);
        throw new InvalidOperationException(
            "Handler not added to the Scoped Handlers! The instance of it is null.");
    }

    /// <summary>
    /// Adds an scoped handler to the updater. ( Use this if you're not sure. )
    /// </summary>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <typeparam name="TUpdate">Update type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="updateType">Update type again.</param>
    /// <param name="filter">A filter to choose the right update.</param>
    /// <param name="getT">
    /// A function to choose real update from <see cref="Update"/>
    /// <para>Don't touch it if you don't know.</para>
    /// </param>
    /// <remarks>This method will add filter attributes if <paramref name="filter"/> is null.</remarks>
    public static IUpdater AddScopedUpdateHandler<THandler, TUpdate>(
        this IUpdater updater,
        Filter<UpdaterFilterInputs<TUpdate>>? filter = default,
        UpdateType? updateType = default,
        Func<Update, TUpdate?>? getT = default,
        int group = default)
        where THandler : IScopedUpdateHandler where TUpdate : class
    {
        if (updateType == null)
        {
            var _t = typeof(TUpdate);
            if (Enum.TryParse(_t.Name, out UpdateType ut))
            {
                updateType = ut;
            }
            else
            {
                throw new InvalidCastException($"{_t} is not an Update! Should be Message, CallbackQuery, ...");
            }
        }

        var _h = typeof(THandler);

        return updater.AddScopedUpdateHandler(
            new ScopedUpdateHandlerContainerBuilder<THandler, TUpdate>(
                updateType.Value, filter, getT), group);
    }

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for any update type that is <see cref="Message"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updateType">
    /// Type of update. should be one of <see cref="UpdateType.Message"/>,
    /// <see cref="UpdateType.EditedMessage"/>, <see cref="UpdateType.ChannelPost"/>,
    /// <see cref="UpdateType.EditedChannelPost"/>.
    /// </param>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdateType updateType,
        Filter<UpdaterFilterInputs<Message>>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, Message>(
            filter, updateType, updateType switch
            {
                UpdateType.Message => x => x.Message,
                UpdateType.EditedMessage => x => x.EditedMessage,
                UpdateType.ChannelPost => x => x.ChannelPost,
                UpdateType.EditedChannelPost => x => x.EditedChannelPost,

                // New updates
                UpdateType.BusinessMessage => x => x.BusinessMessage,
                UpdateType.EditedBusinessMessage => x => x.EditedBusinessMessage,

                _ => throw new ArgumentException(
                    $"Update type {updateType} is not a Message.",
                    nameof(updateType)
                ),
            });

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for any update type that is <see cref="ChatMemberUpdated"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updateType">
    /// Type of update. should be one of
    /// <see cref="UpdateType.ChatMember"/>, <see cref="UpdateType.MyChatMember"/>.
    /// </param>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdateType updateType,
        Filter<UpdaterFilterInputs<ChatMemberUpdated>>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, ChatMemberUpdated>(
            filter, updateType, updateType switch
            {
                UpdateType.ChatMember => x => x.ChatMember,
                UpdateType.MyChatMember => x => x.MyChatMember,

                _ => throw new ArgumentException(
                    $"Update type {updateType} is not a ChatMemberUpdated.",
                    nameof(updateType)
                ),
            });

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.CallbackQuery"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<CallbackQuery>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, CallbackQuery>(
            filter, UpdateType.CallbackQuery, x => x.CallbackQuery);

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.InlineQuery"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<InlineQuery>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, InlineQuery>(
            filter, UpdateType.InlineQuery, x => x.InlineQuery);

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.ChatJoinRequest"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<ChatJoinRequest>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, ChatJoinRequest>(
            filter, UpdateType.ChatJoinRequest, x => x.ChatJoinRequest);

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.ChosenInlineResult"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<ChosenInlineResult>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, ChosenInlineResult>(
            filter, UpdateType.ChosenInlineResult, x => x.ChosenInlineResult);

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.Poll"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<Poll>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, Poll>(
            filter, UpdateType.Poll, x => x.Poll);

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.PollAnswer"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<PollAnswer>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, PollAnswer>(
            filter, UpdateType.PollAnswer, x => x.PollAnswer);

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.PreCheckoutQuery"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<PreCheckoutQuery>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, PreCheckoutQuery>(
            filter, UpdateType.PreCheckoutQuery, x => x.PreCheckoutQuery);

    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.ShippingQuery"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<ShippingQuery>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, ShippingQuery>(
            filter, UpdateType.ShippingQuery, x => x.ShippingQuery);

    // UpdateType.MessageReaction => typeof(MessageReactionUpdated),
    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.MessageReaction"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<MessageReactionUpdated>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, MessageReactionUpdated>(
            filter, UpdateType.MessageReaction, x => x.MessageReaction);

    // UpdateType.MessageReactionCount => typeof(MessageReactionCountUpdated),
    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.MessageReactionCount"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<MessageReactionCountUpdated>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, MessageReactionCountUpdated>(
            filter, UpdateType.MessageReactionCount, x => x.MessageReactionCount);

    // UpdateType.ChatBoost => typeof(ChatBoostUpdated),
    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.ChatBoost"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<ChatBoostUpdated>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, ChatBoostUpdated>(
            filter, UpdateType.ChatBoost, x => x.ChatBoost);

    // UpdateType.RemovedChatBoost => typeof(ChatBoostRemoved),
    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.RemovedChatBoost"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<ChatBoostRemoved>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, ChatBoostRemoved>(
            filter, UpdateType.RemovedChatBoost, x => x.RemovedChatBoost);

    // UpdateType.BusinessConnection => typeof(BusinessConnection),
    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.BusinessConnection"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<BusinessConnection>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, BusinessConnection>(
            filter, UpdateType.BusinessConnection, x => x.BusinessConnection);

    // UpdateType.DeletedBusinessMessages => typeof(BusinessMessagesDeleted),
    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.DeletedBusinessMessages"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<BusinessMessagesDeleted>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, BusinessMessagesDeleted>(
            filter, UpdateType.DeletedBusinessMessages, x => x.DeletedBusinessMessages);

    // UpdateType.PurchasedPaidMedia => typeof(PaidMediaPurchased),
    /// <summary>
    /// Adds an scoped update handler to the updater
    /// for updates of type <see cref="UpdateType.PurchasedPaidMedia"/>.
    /// </summary>
    /// <remarks>
    /// This method will add filter attributes if
    /// <paramref name="filter"/> is <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="updater">The updater.</param>
    /// <param name="filter">The filter.</param>
    public static IUpdater AddScopedUpdateHandler<THandler>(
        this IUpdater updater,
        UpdaterFilter<PaidMediaPurchased>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, PaidMediaPurchased>(
            filter, UpdateType.PurchasedPaidMedia, x => x.PurchasedPaidMedia);
}
