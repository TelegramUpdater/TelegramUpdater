# TelegramUpdater (Preview)

TelegramUpdater is feature-rich framework library for building telegram bots in c#.

## Getting Started

Let's get started with this package and learn how to implement some of above
features.

### Installation

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

The configuration are automatically figured out if they're exists somewhere like `appsettings.json`.
You can add the like this:

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

## What's Next ?!

There are plenty of various examples available at [Examples](https://github.com/TelegramUpdater/TelegramUpdater.Examples)
