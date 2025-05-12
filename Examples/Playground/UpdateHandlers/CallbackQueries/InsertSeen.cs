using Playground.Models;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateHandlers.Controller.Attributes;
using TelegramUpdater.UpdateHandlers.Controller.ReadyToUse;

namespace Playground.UpdateHandlers.CallbackQueries;

/// <summary>
/// Continue to <see cref="Messages.Update"/>.
/// </summary>
[Regex(@"^insertSeen(?:_(?<userId>\d+)_(?<nameCode>.+))?$")]
internal class InsertSeen : CallbackQueryControllerHandler
{
    [HandlerAction]
    public async Task Insert(
        [RegexMatchingGroup("userId")] long userId,
        [RegexMatchingGroup("nameCode")] string nameCode,
        [ResolveService] PlaygroundMemory memory)
    {
        var name = string.Join(' ', nameCode.Split('-'));
        var seenUser = new SeenUser
        {
            TelegramId = userId,
            Name = name,
        };
        await memory.SeenUsers.AddAsync(seenUser);
        await memory.SaveChangesAsync();
        await Edit($"User [{userId}] inserted with name '{name}'");
    }

    [HandlerAction]
    public async Task Abort()
    {
        await Edit($"Ok.");
    }
}
