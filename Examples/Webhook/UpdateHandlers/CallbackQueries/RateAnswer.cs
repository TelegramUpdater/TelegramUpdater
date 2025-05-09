using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace Webhook.UpdateHandlers.CallbackQueries;

[Regex(@"^rating_(?<rate>\d{1})$")]
internal class RateAnswer : CallbackQueryHandler
{
    protected override async Task HandleAsync(CallbackQueryContainer container)
    {
        if (container.TryGetMatchCollection(out var matches))
        {
            var rate = matches[0].Groups["rate"].Value;
            await Edit($"Thank you! Your rating was {rate}.");
            await Answer();
        }
    }
}
