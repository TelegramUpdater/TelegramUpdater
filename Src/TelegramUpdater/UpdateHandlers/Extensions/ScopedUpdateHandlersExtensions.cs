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
    /// <remarks>This method will add filter attributes if <paramref name="filter"/> is null.</remarks>
    public static IUpdater AddScopedUpdateHandler<TUpdate>(
        this IUpdater updater,
        Type typeOfScopedHandler,
        Filter<TUpdate>? filter = default,
        UpdateType? updateType = default,
        Func<Update, TUpdate>? getT = default) where TUpdate : class
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
            containerGeneric, new object?[] { updateType.Value, filter, getT });

        if (container != null)
        {
            return updater.AddScopedUpdateHandler(container);
        }
        else
        {
            updater.Logger.LogWarning(
                "{type} not added to the Scoped Handlers! The instance of it is null.",
                typeOfScopedHandler);
            throw new InvalidOperationException(
                "Handler not added to the Scoped Handlers! The instance of it is null.");
        }
    }

    /// <summary>
    /// Adds an scoped handler to the updater. ( Use this if you'r not sure. )
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
        Filter<TUpdate>? filter = default,
        UpdateType? updateType = default,
        Func<Update, TUpdate?>? getT = default)
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
                updateType.Value, filter, getT));
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
        Filter<Message>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, Message>(
            filter, updateType, updateType switch
            {
                UpdateType.Message => x => x.Message,
                UpdateType.EditedMessage => x => x.EditedMessage,
                UpdateType.ChannelPost => x => x.ChannelPost,
                UpdateType.EditedChannelPost => x => x.EditedChannelPost,
                _ => throw new ArgumentException(
                    $"Update type {updateType} is not a Message."
                )
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
        Filter<ChatMemberUpdated>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, ChatMemberUpdated>(
            filter, updateType, updateType switch
            {
                UpdateType.ChatMember => x => x.ChatMember,
                UpdateType.MyChatMember => x => x.MyChatMember,
                _ => throw new ArgumentException(
                    $"Update type {updateType} is not a ChatMemberUpdated."
                )
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
        Filter<CallbackQuery>? filter = default)
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
        Filter<InlineQuery>? filter = default)
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
        Filter<ChatJoinRequest>? filter = default)
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
        Filter<ChosenInlineResult>? filter = default)
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
        Filter<Poll>? filter = default)
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
        Filter<PollAnswer>? filter = default)
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
        Filter<PreCheckoutQuery>? filter = default)
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
        Filter<ShippingQuery>? filter = default)
        where THandler : IScopedUpdateHandler
        => updater.AddScopedUpdateHandler<THandler, ShippingQuery>(
            filter, UpdateType.ShippingQuery, x => x.ShippingQuery);
}
