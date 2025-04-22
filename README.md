# TelegramUpdater (Preview)

This project aims creating telegram bots in a simplest and more straight way
with DRY.
( _TelegramUpdater aims .NET 6 and above._ )

The package ( should ) handles following:

1. Getting updates from telegram ( Customizable )
2. Ensures updates are processed in parallel.
3. Ensures updates are processed sequential pre each user.
4. Ensures a limited count of updates are processed in parallel. ( This count
is specified as `MaxDegreeOfParallelism`, this ensures that parallel running
tasks should not be more than `MaxDegreeOfParallelism`. )
5. Queuing updates per each user and allows you to semi manage them.
6. Handle updates based on created handlers and filters.
7. Handle exceptions occurred while handling updates.
8. Applies DI ( Dependency Injection ) inside `scoped update Handlers` where an
`IServiceProvider` is available.
9. Enable waiting for other updates while handling an update.
10. Handle overlapping handlers.
11. Some extension methods and properties ( especially in scoped handlers ) to
make faster to develop.
12. Handle some low level staff like handling commands, deep-links and etc.
13. (_Recently_) Helps you keep state of a user in a simplest way.

## Getting Started

Let's get started with this package and learn how to implement some of above
features.

### Installation

The package is available inside
[Nuget](https://www.nuget.org/packages/TelegramUpdater/).

TelegramUpdater uses
[Telegram.Bot: .NET Client for Telegram Bot API](https://github.com/TelegramBots/Telegram.Bot)
package as an C# wrapper over Telegram bot api.

### Setup

Assume you have an empty console application. Put following inside `Program.cs`.

```csharp
using TelegramUpdater;

// Initialize a new instance of Updater using your bot token
var updater = new Updater("<BOT_TOKEN>");

updater.AutoCollectScopedHandlers();
updater.AddDefaultExceptionHandler();

await updater.StartAsync();
```

Let's explain above code.

1. Initialize `Updater`

    To do this you need a Telegram bot token. You can create a bot and get it's
    token from [@BotFather](https://t.me/BotFather).

    Then replace you own token with `<BOT_TOKEN>`.

2. Add you update handlers

    While you can add the update handlers manually using following methods:
    - `updater.AddSingletonUpdateHandler()`
    - `updater.AddScopedUpdaterHandler()`

    But it's better to let's updater fetch them automatically for some reasons:

    1. You won't miss a handler in case you forgot to add it.
    2. You will have all your handler in one place and their right place.
    ...

    **NOTE:**
    - By default, updater will look for handler inside `UpdaterHandlers` namespace
      followed by updater name.

      Eg: `UpdateHandler/Messages` for message handlers.

    - This method looks only for `ScopedUpdateHandler`.

3. Add exception handler

    Method `updater.AddDefaultExceptionHandler()` will add a default `ExceptionHandler`
    that handles all exceptions occurred while handling updates.

4. Start the updater using a default `UpdateWriter`.

    `UpdateWriter` is a class that manages receiving updates from Telegram and
    writing them to the updater.

    You can create your own writer by inheriting from `UpdateWriterAbs`, then
    use `await updater.StartAsync<MyUpdateWriter>()`

Now, if you run the app, you will see some logs and nothing else. Since you should
add some update handlers yet.

### Create update handlers

Let's create a command handler for `/start`.

1. Since we are using `AutoCollectScopedHandlers`, you should create a folder named
`UpdateHandler` ( you can change this naming ) and another folder named `Messages`
inside it.

2. Inside `UpdateHandler/Messages` create a file name `StartCommand.cs`.

    Inside `StartCommand.cs` should be like this:

    ```csharp
    namespace MyApp.UpdateHandlers.Messages;

    internal class StartCommand
    {

    }
    ```

3. Convert your class to an `Scoped message handler`

    Your class should inherit from `MessageHandler`
    ( from `TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse` namespace )

    ```csharp
    using Telegram.Bot.Types;
    using TelegramUpdater.UpdateContainer;
    using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

    namespace YouKnowIKnowYou.UpdateHandlers.Messages;

    internal class StartCommand : MessageHandler
    {
        protected override Task HandleAsync(IContainer<Message> cntr)
        {
            throw new NotImplementedException();
        }
    }
    ```

4. Our implementation to handle the update goes inside `HandleAsync` method.

    For now let's just send a message to the user.

    ```csharp
    protected override async Task HandleAsync(IContainer<Message> _)
    {
        await ResponseAsync("Started from TelegramUpdater");
    }
    ```

5. Add a filter to look for `/start` commands only.

    For scoped update handlers, we use `FilterAttributes` to apply filters.

    ```csharp
    using Telegram.Bot.Types;
    using TelegramUpdater.FilterAttributes.Attributes;
    using TelegramUpdater.UpdateContainer;
    using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

    namespace YouKnowIKnowYou.UpdateHandlers.Messages;

    [Command(command: "start"), Private]
    internal class StartCommand : MessageHandler
    {
        protected override async Task HandleAsync(IContainer<Message> _)
        {
            await ResponseAsync("Started from TelegramUpdater");
        }
    }
    ```

    This is how a real scoped update handler look like.

Now if you run the app and send `/start` to the bot, you'll have a response.

Read more at [Wiki](https://github.com/TelegramUpdater/TelegramUpdater/wiki/1.-Home).

## What's Next ?!

There're plenty of various examples available at [Examples](https://github.com/TelegramUpdater/TelegramUpdater.Examples)
