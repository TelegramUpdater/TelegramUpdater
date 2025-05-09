using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUpdater;
using TelegramUpdater.Hosting;

namespace Webhook.Controllers;

[ApiController]
[Route("[controller]")]
public class BotController(IOptions<UpdaterOptions> Config) : ControllerBase
{
    [HttpGet("setWebhook")]
    public async Task<string> SetWebHook([FromServices] IUpdater updater, CancellationToken ct)
    {
        await updater.SetWebhook(cancellationToken: ct);
        return $"Webhook set to {updater.UpdaterOptions.BotWebhookUrl}";
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update, [FromServices] IUpdater updater, CancellationToken ct)
    {
        if (Request.Headers["X-Telegram-Bot-Api-Secret-Token"] != Config.Value.SecretToken)
            return Forbid();

        // Manually writing the update into updater queue.
        await updater.WriteUpdate(update, ct);
        return Ok();
    }
}