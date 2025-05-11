using Microsoft.EntityFrameworkCore;
using Playground;
using Playground.Models;
using Playground.UpdateHandlers.Messages;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.Hosting;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Minimal;
using TelegramUpdater.UpdateHandlers.Singleton.Attributes;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSqlite<PlaygroundMemory>(
    builder.Configuration.GetConnectionString("DatabaseConnection"));

builder.Services.AddHostedService<UpgradeMemory>();

// this will collect updater options like BotToken, AllowedUpdates and ...
// from configuration section "TelegramUpdater". in this example from appsettings.json
builder.AddTelegramUpdater(
    (builder) => builder

        // Modify the actual updater
        // Don't add scoped handlers here, use the `AddScopedUpdateHandler` method
        // Since this won't add them to the DI container.
        .Execute(updater => updater

            // Add a quick handler
            .Handle(
                UpdateType.Message,
                async (MessageContainer container) =>
                {
                    await container.Response("Want me to help you?!");
                },
                ReadyFilters.OnCommand("help"))

            .Handle(
                UpdateType.Message,
                async (IContainer<Message> container, PlaygroundMemory memory) =>
                {
                    var records = await memory.SeenUsers.CountAsync();
                    await container.Response($"I've seen {records} people so far.");
                },
                ReadyFilters.OnCommand("records") & ReadyFilters.PM())

            .Handle(
                UpdateType.Message,
                async (IContainer<Message> container, PlaygroundMemory memory) =>
                {
                    if (container.TryParseCommandArgs(out long? id))
                    {
                        if (await memory.SeenUsers
                            .SingleOrDefaultAsync(x => x.TelegramId == id) is SeenUser seen)
                        {
                            await container.Response($"User {seen.Name} [{seen.Id}] is seen.");
                        }
                        else
                        {
                            await container.Response($"User {id} is not seen.");
                        }
                    }
                    else
                    {
                        await container.Response("I need an integer user id to check.");
                    }
                },
                ReadyFilters.OnCommand("check"))

            // Collects static methods marked with `SingletonHandlerCallback` attribute.
            .CollectHandlingCallbacks()

            // State tracker
            .AddUserEnumStateKeeper<RenameState>())

        // Manually add scoped handlers (while registering into di)
        .AddMessageHandler<MyBadlyPlacedHandler>()

        // Collect scoped handlers located for example at UpdateHandlers/Messages for messages.
        .CollectHandlers()
        .AddDefaultExceptionHandler());

var host = builder.Build();
await host.RunAsync();

partial class Program
{
    /// <summary>
    /// This method is automatically collected and considered as an singleton update handler.
    /// You just need to call <see cref="SingletonAttributesExtensions.CollectHandlingCallbacks(IUpdater)"/>
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    [Command("ask"), Private]
    [HandlerCallback(UpdateType.Message)]
    public static async Task AboutCommand(IContainer<Message> container)
    {
        var message = await container.Response("Wanna know more about me?! Answer right now!",
            replyMarkup: new InlineKeyboardMarkup([[
                InlineKeyboardButton.WithCallbackData("Yes"),
                InlineKeyboardButton.WithCallbackData("No")]]));

        // Wait for short coming answer right here
        var answer = await container.ChannelButtonClick(
            TimeSpan.FromSeconds(5),
            new(@"Yes|No"));

        switch (answer)
        {
            case { Update.Data: { } data }:
                {
                    // User did answer
                    if (data == "Yes")
                        await answer.Edit("Well me too :)");
                    else
                        await answer.Edit("Traitor!");

                    await answer.Answer();

                    break;
                }
            default:
                {
                    // Likely timed out.
                    await message.Edit("Slow");
                    break;
                }
        }
    }
}