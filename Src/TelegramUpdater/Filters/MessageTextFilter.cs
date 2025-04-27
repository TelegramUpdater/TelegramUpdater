namespace TelegramUpdater.Filters;

/// <summary>
/// A filter on <see cref="Message.Text"/>
/// </summary>
public class MessageTextFilter(Func<string, bool> filter)
    : UpdaterFilter<Message>((x) => x.Text != null && filter(x.Text))
{
}
