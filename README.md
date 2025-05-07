# TelegramUpdater (Preview)

TelegramUpdater is feature-rich framework library for building telegram bots in c#.

## Features

- Telegram updater manages update handling process from receiving updates to
queuing them filtering them, answering them, handling exceptions, managing state and more ...
- Updater ensures that only a limited number of updates are begin processed in parallel,
and if the user id can be figured out, the updates are processed in sequence per each user.
- Benefiting from queuing the incoming updates, you can have access to the future by `ReadNext`.
- Singleton and Scoped handlers which can enable you to use DI and scoped services.
- Rich filter system by allowing to combine filters or applying them as attributes.
- Containers with access to a lot of resources like `Update` and inner update, `Updater` itself and more. This can enable you to have,
use or create a lot of extension methods on them.
- Update channels can make a channel from your current handler to the updating pipeline and fetch
future updates, like waiting for text input right inside the handler.
- Handling exceptions whiling filtering them by type or message.
- Hosting support as mentioned before, to have access to DI and webhook.
- Managing state and integrating them with filters and container's extension methods.
- Many helper (extension) methods and types. 
- Super abstraction over any thing! You can create your own filter, handler, container, update channels, extension methods and even update writers.

## Getting Started

Let's get started with this package and learn how to implement some of above
features.

## Installation

The package is available inside
[Nuget](https://www.nuget.org/packages/TelegramUpdater/).

TelegramUpdater uses
[Telegram.Bot: .NET Client for Telegram Bot API](https://github.com/TelegramBots/Telegram.Bot)
package as an C# wrapper over Telegram bot api.

### Code sample

#### Console app

A very minimal yet working example of TelegramUpdater usage is something like this.

```csharp
using TelegramUpdater;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

var updater = new Updater("YOUR_BOT_TOKEN")
    .AddDefaultExceptionHandler()
    .AddMessageHandler(
        async (MessageContainer container) =>
        {
            await container.Response("Hello World");
        },
        ReadyFilters.OnCommand("start") & ReadyFilters.PM()
    );

await updater.Start();
```

This setups `Updater` with your bot token, adds a default exception handler that logs errors,
a (singleton) update handler that works on `/start` command on private chats
and finally starts up the updater.

Updater can automatically collect your handlers as statics methods like example below

```csharp
var updater = new Updater("YOUR_BOT_TOKEN")
    .AddDefaultExceptionHandler()
    .CollectSingletonUpdateHandlerCallbacks();

await updater.Start();

partial class Program
{
    [Command("start"), Private]
    [SingletonHandlerCallback(UpdateType.Message)]
    public static async Task Start(MessageContainer container)
    {
        await container.Response("Hello World");
    }
}
```

This should work the same as before. (Filters are applied as attributes)

#### Worker service

If your project is a worker service or anything that has HostBuilder and DI (dependency injection)
in it, you can setup updater like this.

```csharp
var builder = Host.CreateApplicationBuilder(args);

// this will collect updater options like BotToken, AllowedUpdates and ...
// from configuration section "TelegramUpdater". in this example from appsettings.json
builder.AddTelegramUpdater(
    (builder) => builder
        // Modify the actual updater
        .Execute(updater => updater
            // Collects static methods marked with `SingletonHandlerCallback` attribute.
            .CollectSingletonUpdateHandlerCallbacks())
        // Collect scoped handlers located for example at UpdateHandlers/Messages for messages.
        .AutoCollectScopedHandlers()
        .AddDefaultExceptionHandler());

var host = builder.Build();
await host.RunAsync();
```

The configuration are automatically figured out if they're exists somewhere like in `appsettings.json`.
You can add them like this:

```json
{
  
  ...

  "TelegramUpdater": {
    "BotToken": "",
    "AllowedUpdates": [ "Message", "CallbackQuery" ]
  },
  
  ...
}
```

**Note**: Updater can and will figure out `AllowedUpdates` if not specified by looking
into you registered handlers.

For singleton handlers it's just like before, but if your going to use scoped handlers,
put them into the right place as mentioned in the example.

For example create a file at `UpdateHandlers/Messages` like `UpdateHandlers/Messages/Start.cs`

The `Start.cs` should look like this:

```csharp
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace GettingStarted.UpdateHandlers.Messages;

[Command("start"), Private]
internal class Start : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer container)
    {
        await container.Response("Hello World");
    }
}
```

Watch out for name space where the `MessageHandler` came from, it must be `...Scoped.ReadyToUse` not `Singleton`.

And the filters are now applied on class.

The handler will be automatically collected by the updater if you call `AutoCollectScopedHandlers`.
An now you can use your `IFancyService` which is available in DI right into `Start`'s constructor.

### Something cool?!

```csharp
[Command("about"), Private]
[SingletonHandlerCallback(UpdateType.Message)]
public static async Task AboutCommand(IContainer<Message> container)
{
    var message = await container.Response("Wanna know more about me?! Answer right now!",
        replyMarkup: new InlineKeyboardMarkup([[
            InlineKeyboardButton.WithCallbackData("Yes"),
            InlineKeyboardButton.WithCallbackData("No")]]));

    // Wait for short coming answer right here
    var answer = await container.ChannelButtonClick(TimeSpan.FromSeconds(5), new(@"Yes|No"));

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
```

Extension methods return containerized results.

## What's Next ?!

There are plenty of various examples available at [Examples](https://github.com/TelegramUpdater/TelegramUpdater.Examples)
