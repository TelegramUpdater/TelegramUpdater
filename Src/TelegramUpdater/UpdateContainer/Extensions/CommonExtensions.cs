using System.Linq.Expressions;
using TelegramUpdater.RainbowUtilities;
using TelegramUpdater.UpdateContainer.UpdateContainers;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TelegramUpdater.UpdateContainer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// A set of common extension.
/// </summary>
public static class CommonExtensions
{
    internal static RawContainer RebaseAsRaw<T>(
        this IContainer<T> container, ShiningInfo<long, Update> shiningInfo) where T : class
        => new(new(container.Updater, shiningInfo, default, default!, default, default));

    internal static async Task<IBaseContainer<T>> WrapAsync<T>(
        this Task<T> t, Expression<Func<Update, T?>> insiderResovler, IUpdater updater) where T : class
    {
        var result = await t.ConfigureAwait(false);
        return DefaultLiteContainer<T>.CreateLiteContainer(insiderResovler, result, updater);
    }

    internal static IBaseContainer<T> Wrap<T>(
        this T t, Expression<Func<Update, T?>> insiderResovler, IUpdater updater) where T : class
    {
        return DefaultLiteContainer<T>.CreateLiteContainer(insiderResovler, t, updater);
    }

    internal static async Task<IBaseContainer<Message>> WrapMessageAsync(
        this Task<Message> t, IUpdater updater)
    {
        return await t.WrapAsync(x => x.Message, updater).ConfigureAwait(false);
    }
}
