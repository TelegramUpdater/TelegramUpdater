using System.Linq.Expressions;
using System.Reflection;
using TelegramUpdater.RainbowUtilities;

namespace TelegramUpdater.UpdateContainer.UpdateContainers;

/// <summary>
/// Lite container to use for updates that are received
/// outside of updaters. Eg: the result of requests.
/// </summary>
/// <typeparam name="T"></typeparam>
public class AnyLiteContainer<T> : UpdateContainerAbs<T> where T : class
{
    /// <summary>
    /// Create a lite container.
    /// </summary>
    /// <param name="insiderResolver">
    /// A function to resolve inner update.
    /// </param>
    /// <param name="update">The update itself.</param>
    /// <param name="updater">The updater instance</param>
    /// <param name="extraObjects"></param>
    internal AnyLiteContainer(
        Func<Update, T?> insiderResolver,
        Update update,
        IUpdater updater,
        IReadOnlyDictionary<string, object>? extraObjects = default)
        : base(insiderResolver,
               updater,
               insider: update,
               extraObjects: extraObjects)
    { }


    /// <inheritdoc/>
    public override ShiningInfo<long, Update> ShiningInfo
        => throw new InvalidOperationException(
            "Lite contianers have no ShiningInfo, since they're not received from updater.");

    internal static IContainer<U> CreateLiteContainer<U>(
        Expression<Func<Update, U?>> insiderResolver,
        U update,
        IUpdater updater) where U : class
    {
        var u = new Update();
        var prop = (PropertyInfo)((MemberExpression)insiderResolver.Body)
            .Member;

        prop.SetValue(u, update);

        return new AnyLiteContainer<U>(
            insiderResolver.Compile(), u, updater);
    }

    internal static IContainer<Message> MessageLiteContainer(
        Message update, IUpdater updater)
        => CreateLiteContainer(x => x.Message, update, updater);

    internal static IContainer<CallbackQuery> CallbackQueryLiteContainer(
        CallbackQuery update, IUpdater updater)
        => CreateLiteContainer(x => x.CallbackQuery, update, updater);
}
