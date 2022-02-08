using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using TelegramUpdater;
using TelegramUpdater.Asp;

namespace WebhookApp.Controllers
{
    public class WebhookController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromServices] Updater updater,
                                              [FromBody] Update update)
        {
            await updater.WriteUpdateAsync(update);
            return Ok();
        }
    }
}
