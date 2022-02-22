﻿using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateContainer;

public static class CommonExtensions
{
    public static RawContainer RebaseAsRaw<T>(
        this IContainer<T> container, ShiningInfo<long, Update> shiningInfo) where T : class
        => new(container.Updater, shiningInfo);

    internal static async Task<IContainer<T>> WrapAsync<T>(
        this Task<T> t, Expression<Func<Update, T?>> insiderResovler, IUpdater updater) where T : class
    {
        var result = await t;
        return AnyLiteContainer<T>.CreateLiteContainer(insiderResovler, result, updater);
    }

    internal static IContainer<T> Wrap<T>(
        this T t, Expression<Func<Update, T>> insiderResovler, IUpdater updater) where T : class
    {
        return AnyLiteContainer<T>.CreateLiteContainer(insiderResovler, t, updater);
    }

    internal static async Task<IContainer<Message>> WrapMessageAsync(
        this Task<Message> t, IUpdater updater)
    {
        return await t.WrapAsync(x => x.Message, updater);
    }
}