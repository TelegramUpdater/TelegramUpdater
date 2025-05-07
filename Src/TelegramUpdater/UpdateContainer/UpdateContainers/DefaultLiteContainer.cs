using System.Linq.Expressions;
using System.Reflection;
using TelegramUpdater.RainbowUtilities;

namespace TelegramUpdater.UpdateContainer.UpdateContainers;

/// <summary>
/// Lite container to use for updates that are received
/// outside of updater. Eg: the result of requests.
/// </summary>
/// <typeparam name="T"></typeparam>
public class DefaultLiteContainer<T> : IContainer<T> where T : class
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
        var input = new HandlerInput(
            updater,
            new ShiningInfo<long, Update>(update, default!, default),
            default, default, default, default);

        Input = input;
        Update = insiderResolver(update) 
            ?? throw new InvalidOperationException("Inner update can't be null.");
    }

    /// <inheritdoc/>
    public ShiningInfo<long, Update> ShiningInfo
        => throw new InvalidOperationException(
            "Lite containers have no ShiningInfo, since they're not received from updater.");

    /// <inheritdoc />
    public T Update { get; }

    HandlerInput IContainer.Input => Input;

    ShiningInfo<long, Update> IContainer.ShiningInfo
        => throw new InvalidOperationException(
            "Lite containers have no ShiningInfo, since they're not received from updater.");

    object IContainer.this[string key]
        => throw new InvalidOperationException("Lite container doesn't have any extra data.");

    /// <inheritdoc />
    public HandlerInput Input { get; }

    internal static IContainer<U> CreateLiteContainer<U>(
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

    internal static IContainer<Message> MessageLiteContainer(
        Message update, IUpdater updater)
        => CreateLiteContainer(x => x.Message, update, updater);

    internal static IContainer<CallbackQuery> CallbackQueryLiteContainer(
        CallbackQuery update, IUpdater updater)
        => CreateLiteContainer(x => x.CallbackQuery, update, updater);

    bool IContainer.ContainsKey(string key)
    {
        throw new InvalidOperationException("Lite container doesn't have any extra data.");
    }
}
