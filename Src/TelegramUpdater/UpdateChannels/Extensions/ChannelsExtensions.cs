using Telegram.Bot.Types.Payments;
using TelegramUpdater.Filters;
using TelegramUpdater.Filters.Extensions;
using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateChannels;
using TelegramUpdater.UpdateChannels.ReadyToUse;
using TelegramUpdater.UpdateContainer.UpdateContainers;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TelegramUpdater.UpdateContainer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// A set of extension methods for <see cref="IUpdateChannel"/>s.
/// </summary>
public static class ChannelsExtensions
{
    /// <summary>
    /// Opens a channel and waits for an specified update to come.
    /// </summary>
    /// <remarks>
    /// Using channels requires an update that with some sort of user id.
    /// </remarks>
    /// <typeparam name="TUpdate">Update type you're expecting.</typeparam>
    /// <param name="container">The container itself.</param>
    /// <param name="updateChannel">
    /// Your channel to choose the right update.
    /// </param>
    /// <param name="onUnrelatedUpdate">
    /// A callback function to be called if an unrelated update from comes.
    /// </param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public static async ValueTask<IContainer<TUpdate>?> OpenChannel<TUpdate>(
        this IContainer container,
        IGenericUpdateChannel<TUpdate> updateChannel,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
        where TUpdate : class
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(container);
#else
        if (container is null)
            throw new ArgumentNullException(nameof(container));
#endif

        if (updateChannel == null)
        {
            throw new InvalidOperationException(
                "abstractChannel and updateResolver both can't be null");
        }

        // A secondary timeOut, because ReadNextAsync'timeout will reset on unrelated update.
        var timeOutCts = new CancellationTokenSource();
        timeOutCts.CancelAfter(updateChannel.TimeOut);

        using var linkedCts = CancellationTokenSource
            .CreateLinkedTokenSource(timeOutCts.Token, cancellationToken);

        while (true)
        {
            try
            {
                var update = await container.ShiningInfo.ReadNextAsync(
                    updateChannel.TimeOut, linkedCts.Token).ConfigureAwait(false);

                if (update == null)
                    return null;

                if (updateChannel.ShouldChannel(container.GetFilterInputs(update.Value)))
                {
                    return new DefaultContainer<TUpdate>(
                        updateChannel.GetActualUpdate,
                        container.Input.WithNewShiningInfo(update),
                        updateChannel.ExtraData
                    );
                }

                if (onUnrelatedUpdate != null)
                {
                    await onUnrelatedUpdate(container.Updater, update).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                if (timeOutCts.IsCancellationRequested) return default;
                throw;
            }
        }
    }

    /// <summary>
    /// Opens a channel and waits for an specified update to come.
    /// </summary>
    /// <remarks>
    /// Using channels requires an update that with some sort of user id.
    /// </remarks>
    /// <param name="container"></param>
    /// <param name="updateType">Type of the update.</param>
    /// <param name="timeOut">Time out for how much time should channel wait for an incoming update.</param>
    /// <param name="filter">Filter to choose the right update to channel.</param>
    /// <param name="onUnrelatedUpdate">
    /// A callback function to be called if an unrelated updates comes.
    /// <para>This means an update of the same type but not matching with filters.</para>
    /// </param>
    /// <param name="resolveInnerUpdate">Optionally resolve <typeparamref name="TUpdate"/> from an <see cref="Update"/>.</param>
    /// <param name="cancellationToken">To cancel the channel before <paramref name="timeOut"/>.</param>
    public static ValueTask<IContainer<TUpdate>?> OpenChannel<TUpdate>(
        this IContainer container,
        UpdateType updateType,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<TUpdate>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        Func<Update, TUpdate?>? resolveInnerUpdate = default,
        CancellationToken cancellationToken = default)
        where TUpdate : class
    {
        var defaultChannel = new DefaultChannel<TUpdate>(updateType, timeOut, resolveInnerUpdate, filter);
        return container.OpenChannel(defaultChannel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<ChatMemberUpdated>?> OpenChannel(
        this IContainer container,
        UpdateType updateType,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<ChatMemberUpdated>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        IGenericUpdateChannel<ChatMemberUpdated> channel = updateType switch
        {
            UpdateType.ChatMember => new ChatMemberChannel(timeOut, filter),
            UpdateType.MyChatMember => new MyChatMemberChannel(timeOut, filter),
            _ => throw new InvalidOperationException(
                $"Input update type {updateType} dose not match with {typeof(ChatMemberUpdated)}"),
        };

        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<InlineQuery>?> OpenChannel(
        this IContainer container,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<InlineQuery>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        var channel = new InlineQueryChannel(timeOut, filter);
        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<ChosenInlineResult>?> OpenChannel(
        this IContainer container,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<ChosenInlineResult>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        var channel = new ChosenInlineResultChannel(timeOut, filter);
        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<CallbackQuery>?> OpenChannel(
        this IContainer container,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<CallbackQuery>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        var channel = new CallbackQueryChannel(timeOut, filter);
        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<ShippingQuery>?> OpenChannel(
        this IContainer container,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<ShippingQuery>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        var channel = new ShippingQueryChannel(timeOut, filter);
        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<PreCheckoutQuery>?> OpenChannel(
        this IContainer container,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<PreCheckoutQuery>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        var channel = new PreCheckoutQueryChannel(timeOut, filter);
        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<Poll>?> OpenChannel(
        this IContainer container,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<Poll>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        var channel = new PollChannel(timeOut, filter);
        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<PollAnswer>?> OpenChannel(
        this IContainer container,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<PollAnswer>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        var channel = new PollAnswerChannel(timeOut, filter);
        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<ChatJoinRequest>?> OpenChannel(
        this IContainer container,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<ChatJoinRequest>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        var channel = new ChatJoinRequestChannel(timeOut, filter);
        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<MessageReactionUpdated>?> OpenChannel(
        this IContainer container,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<MessageReactionUpdated>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        var channel = new MessageReactionChannel(timeOut, filter);
        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<MessageReactionCountUpdated>?> OpenChannel(
        this IContainer container,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<MessageReactionCountUpdated>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        var channel = new MessageReactionCountChannel(timeOut, filter);
        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<ChatBoostUpdated>?> OpenChannel(
        this IContainer container,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<ChatBoostUpdated>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        var channel = new ChatBoostChannel(timeOut, filter);
        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<ChatBoostRemoved>?> OpenChannel(
        this IContainer container,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<ChatBoostRemoved>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        var channel = new RemovedChatBoostChannel(timeOut, filter);
        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<BusinessConnection>?> OpenChannel(
        this IContainer container,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<BusinessConnection>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        var channel = new BusinessConnectionChannel(timeOut, filter);
        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<BusinessMessagesDeleted>?> OpenChannel(
        this IContainer container,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<BusinessMessagesDeleted>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        var channel = new DeletedBusinessMessagesChannel(timeOut, filter);
        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <inheritdoc cref="OpenChannel{T}(IContainer, UpdateType, TimeSpan, IFilter{UpdaterFilterInputs{T}}?, Func{IUpdater, ShiningInfo{long, Update}, Task}?, Func{Update, T}, CancellationToken)"/>
    public static ValueTask<IContainer<PaidMediaPurchased>?> OpenChannel(
        this IContainer container,
        TimeSpan timeOut,
        IFilter<UpdaterFilterInputs<PaidMediaPurchased>>? filter = default,
        Func<
            IUpdater, ShiningInfo<long, Update>,
            Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        var channel = new PurchasedPaidMediaChannel(timeOut, filter);
        return container.OpenChannel(channel, onUnrelatedUpdate, cancellationToken);
    }

    /// <summary>
    /// Opens a channel that dispatches a <see cref="Message"/> from updater.
    /// </summary>
    /// <param name="updateContainer">The update container</param>
    /// <param name="timeOut">Maximum allowed time to wait for the update.
    /// <para>
    /// 30 sec. by default.
    /// </para>
    /// </param>
    /// <param name="filter">Filter updates to get the right one.</param>
    /// <param name="onUnrelatedUpdate">
    /// A callback function to be called if an unrelated update from comes.
    /// </param>
    /// <param name="cancellationToken">To cancel the job.</param>
    public static async Task<IContainer<Message>?> ChannelMessage(
        this IContainer updateContainer,
        Filter<UpdaterFilterInputs<Message>>? filter,
        TimeSpan? timeOut,
        Func<
            IUpdater,
            ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        return await updateContainer.OpenChannel(
            new MessageChannel(timeOut ?? TimeSpan.FromSeconds(30), filter),
            onUnrelatedUpdate,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Waits for the user to click on an inline button.
    /// </summary>
    public static async Task<IContainer<CallbackQuery>?> ChannelButtonClick(
        this IContainer updateContainer,
        TimeSpan timeOut,
        CallbackQueryRegex callbackQueryRegex,
        Func<
            IUpdater,
            ShiningInfo<long, Update>, Task>? onUnrelatedUpdate = default,
        CancellationToken cancellationToken = default)
    {
        return await updateContainer.OpenChannel(
            new CallbackQueryChannel(
                timeOut,
                callbackQueryRegex),
            onUnrelatedUpdate,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
