using System.Linq.Expressions;
using System.Reflection;

namespace TelegramUpdater.UpdateContainer.UpdateContainers;

/// <summary>
/// Lite container to use for updates that are received
/// outside of updater. Eg: the result of requests.
/// </summary>
/// <typeparam name="T"></typeparam>
public class DefaultLiteContainer<T> : IBaseContainer<T> where T : class
{
    /// <summary>
    /// Create a lite container.
    /// </summary>
    /// <param name="insiderResolver">
    /// A function to resolve inner update.
    /// </param>
    /// <param name="updater"></param>
    /// <param name="update"></param>
    internal DefaultLiteContainer(
        Func<Update, T?> insiderResolver,
        IUpdater updater,
        Update update)
    {
        Updater = updater;
        Container = update;
        Update = insiderResolver(update)
            ?? throw new InvalidOperationException("Inner update can't be null.");
    }

    /// <inheritdoc />
    public T Update { get; }

    /// <inheritdoc />
    public IUpdater Updater { get; }

    /// <inheritdoc />
    public Update Container { get; }

    internal static IBaseContainer<U> CreateLiteContainer<U>(
        Expression<Func<Update, U?>> insiderResolver,
        U update,
        IUpdater updater) where U : class
    {
        var u = new Update();
        var prop = (PropertyInfo)((MemberExpression)insiderResolver.Body)
            .Member;

        prop.SetValue(u, update);

        return new DefaultLiteContainer<U>(
            insiderResolver.Compile(), updater, u);
    }

    internal static IBaseContainer<Message> MessageLiteContainer(
        Message update, IUpdater updater)
        => CreateLiteContainer(x => x.Message, update, updater);

    internal static IBaseContainer<CallbackQuery> CallbackQueryLiteContainer(
        CallbackQuery update, IUpdater updater)
        => CreateLiteContainer(x => x.CallbackQuery, update, updater);
}
