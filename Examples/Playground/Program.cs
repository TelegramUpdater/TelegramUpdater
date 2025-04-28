using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.Hosting;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Singleton.Attributes;

var builder = Host.CreateApplicationBuilder(args);

var botToken = builder.Configuration.GetSection("TelegramUpdater:BotToken")
    .Get<string>() ?? throw new InvalidOperationException("Bot token not found.");

builder.Services.AddTelegramUpdater(
    botToken,
    new UpdaterOptions(
        allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery]),

    (builder) => builder
        .Execute(updater => updater
            // Add in line handler
            .AddSingletonUpdateHandler(
                UpdateType.Message,
                async (container) =>
                {
                    await container.Response("Want me to help you?!");
                },
                FilterCutify.OnCommand("help"))
            // Collects static methods marked with `SingletonHandlerCallback` attribute.
            .CollectSingletonUpdateHandlerCallbacks())
        // Collect scoped handlers located for example at UpdateHandlers/Messages for messages.
        .AutoCollectScopedHandlers()
        .AddDefaultExceptionHandler());

var host = builder.Build();
await host.RunAsync();


partial class Program
{
    /// <summary>
    /// This method is automatically collected and considered as an singleton update handler.
    /// You just need to call <see cref="SingletonAttributesExtensions.CollectSingletonUpdateHandlerCallbacks(IUpdater)"/>
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    [Command("about"), Private]
    [SingletonHandlerCallback(UpdateType.Message)]
    public static async Task AboutCommand(IContainer<Message> container)
    {
        var message = await container.Response("Wanna know more about me?! Answer right now!",
            replyMarkup: new InlineKeyboardMarkup([[
                InlineKeyboardButton.WithCallbackData("Yes"),
                InlineKeyboardButton.WithCallbackData("No")]]));

        var answer = await container.ChannelButtonClick(
            TimeSpan.FromSeconds(5),
            new(@"Yes|No"));

        if (answer == null) // likely Timed out.
        {
            await message.Edit("Slow");
            return;
        }

        switch (answer)
        {
            case { Update.Data: { } data }:
                {
                    if (data == "Yes")
                        await answer.Edit("Well me too :)");
                    else
                        await answer.Edit("Traitor!");

                    await answer.Answer();

                    break;
                }
        }
    }
}