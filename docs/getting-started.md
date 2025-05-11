# Getting Started

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
