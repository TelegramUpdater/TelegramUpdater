namespace TelegramUpdater.Filters;

/// <summary>
/// A filter on <see cref="Message.Text"/>
/// </summary>
public class MessageTextFilter(Func<string, bool> filter)
    : Filter<Message>((_, x) => x.Text != null && filter(x.Text))
{
}
