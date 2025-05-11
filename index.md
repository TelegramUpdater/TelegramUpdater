---
_layout: landing
---

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
    .QuickStartCommandReply("Hello there!");

await updater.Start();
```

This setups `Updater` with your bot token, adds a default exception handler that logs errors,
a (singleton) update handler that works on `/start` command on private chats
and finally starts up the updater.

Updater can automatically collect your handlers as statics methods like example below

```csharp
var updater = new Updater("YOUR_BOT_TOKEN")
    .AddDefaultExceptionHandler()
    .CollectHandlingCallbacks();

await updater.Start();

partial class Program
{
    [Command("start"), Private]
    [HandlerCallback(UpdateType.Message)]
    public static async Task Start(IContainer<Message> container)
    {
        await container.Response("Hello World");
    }
}
```

This should work the same as before. (Filters are applied as attributes)

> [!WARNING]
> If you add scoped handlers but your `Updater` without having access to the DI (`IServiceProvider`), the updater will still try to make an instance of you scoped handler
> if its filters passes, by its parameterless constructor.

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
            // Collects static methods marked with `HandlerCallback` attribute.
            .CollectHandlingCallbacks())
        // Collect scoped handlers located for example at UpdateHandlers/Messages for messages.
        .CollectHandlers()
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
> [!NOTE]
> Updater can and will figure out `AllowedUpdates` if not specified by looking
> into you registered handlers.

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

The handler will be automatically collected by the updater if you call `CollectHandlers`.
An now you can use your `IFancyService` which is available in DI right into `Start`'s constructor.

Instead of using `HandleAsync(MessageContainer container)`, you use a more contextual overload like this:

``` csharp
HandleAsync(
    MessageContainer input,
    IServiceScope? scope,
    CancellationToken cancellationToken)
```

But remember not to override both! (The one with less parameters will be ignored).

### Minimals

Instead using scoped handlers as classes you can also create your handlers Minimally!

```csharp

// This is a minimal handler
updater.Handle(
    UpdateType.Message,
    async (IContainer<Message> container, IFancyService service) =>
    {
        // Do something with the container and service
    },
    ReadyFilters.OnCommand("command"))

```

> [!NOTE]
> Typically Methods like `updater.Handle(...)` refers to a singleton handler
> and `updater.AddHandler(...)` or  `updater.Add...Handler` refer to an scoped handler.
> Minimal handlers are actually singleton handlers but you can use DI inside them.

### Something cool?!

```csharp
[Command("about"), Private]
[HandlerCallback(UpdateType.Message)]
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

> [!CAUTION]
> The package is still in preview and the API may change in the future *(for sure)*.
> It's actually changing right now, so be careful when using it.

## Examples

Examples inside [/Examples](https://github.com/TelegramUpdater/TelegramUpdater/tree/master/Examples) folder are up to date with latest package changes and are good start points to begin.

- [ConsoleApp](https://github.com/TelegramUpdater/TelegramUpdater/tree/master/Examples/Examples/ConsoleApp): Usage of updater inside a console app.
- [WorkerService](https://github.com/TelegramUpdater/TelegramUpdater/tree/master/Examples/Examples/WorkerService): Usage of the updater inside a worker service where `IServiceCollection`, `IConfiguration`s can be used by the updater (This's preferred to the console app as you can use scoped handlers)
- [Webhook](https://github.com/TelegramUpdater/TelegramUpdater/tree/master/Examples/Examples/Webhook): Setting up a telegram bot using webhooks and updater. (Most of this are as same as WorkerSerivce)
- [Playground](https://github.com/TelegramUpdater/TelegramUpdater/tree/master/Examples/Examples/Playground): This is a good worker service example that uses many of TelegramUpdater's features (not all!).

## What's Next ?!

The package has also some extension packages:

- [TelegramUpdater.FillMyForm](https://github.com/TelegramUpdater/TelegramUpdater.FillMyForm): which magically fills your forms for you.
- [TelegramUpdater.Menu](https://github.com/TelegramUpdater/TelegramUpdater.Menu): Work with static menus (InlineKeyboards and more).

## Help is always needed!

If you think this package worths it, then help and contribute. It will be always appreciated!
