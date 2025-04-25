namespace TelegramUpdater.Filters;

/// <summary>
/// A filter on <see cref="CallbackQuery.Data"/>
/// </summary>
/// <remarks>
/// A filter on <see cref="CallbackQuery.Data"/>
/// </remarks>
public class CallbackQueryDataFilter(Func<string, bool> filter)
    : Filter<CallbackQuery>((_, x) => x.Data != null && filter(x.Data))
{
}
