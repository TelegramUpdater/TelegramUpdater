# Here is **Updater**

** !! Preview !! **

This is your telegram updater package written in C# and .NET 6

The updater is supposed to fetch and handle new updates coming from bot api server

The updater is written on top of 
[TelegramBots/Telegram.Bot: .NET Client for Telegram Bot API](https://github.com/TelegramBots/Telegram.Bot) package

## More support

We can talk through [@TUTalkings](https://t.me/TUTalkings) if you want to.

## Why use this?

- Updater uses update handlers which are a great help to write clean, simple to write and read and more powerful code base.
- Updater provides `Filter<T>` class that helps you to easily choose the right handler for incoming updates.
Take a look at code below describing a handler:

```csharp
using Telegram.Bot.Types;
using TelegramUpdater;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.FilterAttributes.Attributes;
using TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse;

namespace ConsoleApp;

[Command("test"), Private]
internal class MyScopedMessageHandler : ScopedMessageHandler
{
    protected override async Task HandleAsync(IContainer<Message> _)
    {
        await ResponseAsync("Tested!");
    }
}
```

- Simple setup
- Useful options
```cs
updater = new Updater("BotToken",
    new UpdaterOptions(
        maxDegreeOfParallelism: 10, // maximum update process tasks count at the same time
                                    // Eg: first 10 updates are answers quickly, but others should wait
                                    // for any of that 10 to be done.
                                    
    .AddScopedUpdateHandler<MyScopedMessageHandler, Message>(); // Scoped handler;

await updater.StartAsync(); // 🔥 Fire up and block!
```

- `OpenChannel` Method! You can use this to wait for a user response.
- Batch of extension methods to increase speed and make cleaner code.

As instance `ChannelUserClick` is an helper method for `OpenChannel` that waits for a user click.

```cs
[Command("start"), Private]
internal sealed class MyMessageHandler : MessageHandler
{
    protected override async Task HandleAsync(IContainer<Message> _)
    {
        await AwaitButtonClickAsync(
            TimeSpan.FromSeconds(10), new CallbackQueryRegex("^hello"))

        .IfNotNull(async answer =>
        {
            await answer.EditAsync(text: "Well ...");
        })
        .Else(async _ =>
        {
            await ResponseAsync("Slow");
        });
    }
}
```

- `ExceptionHandler`s, handle exceptions like a pro with highly customizable exceptions handlers. you specify exception type, message match, handler type and ...

```cs
    .StepTwo(CommonExceptions.ParsingException(
        (updater, ex) =>
        {
            updater.Logger.LogWarning(exception: ex, "Handler has entity parsing error!");
            return Task.CompletedTask;
        },
        allowedHandlers: new[]
        {
            typeof(AboutMessageHandler),
            typeof(MyScopedMessageHandler)
        }))
```

- Supports DI and batch of extension methods for hosted or webhook apps ( thanks to Telegram.Bot webhook example )
- Updater has a lot of base classes, interfaces and generic types to make everything highly customizable.
- More ...

## Getting Started

Here are starting pack for common SDKs in .NET

> TelegramUpdater in available in [nuget](https://www.nuget.org/packages/TelegramUpdater/), Install it first.

### Basic

If you're using a console app with no Hosting and `IServiceCollection` then it's your choice
**And even if you don't, you're suggested to!**

Base class of this package is `Updater`, but there's a helper class in case of basic apps called `UpdaterBuilder` which helps you
get familiar with the package.

`UpdaterBuilder` helps you build `Updater` in steps with fully documented methods.
See [UpdaterProduction](https://github.com/TelegramUpdater/TelegramUpdater/tree/master/Examples/UpdaterProduction) and
[ConsoleApp](https://github.com/TelegramUpdater/TelegramUpdater/tree/master/Examples/ConsoleApp) for instance.

If you're looking for a quick basic example:

```csharp
// See https://aka.ms/new-console-template for more information
using TelegramUpdater;

var updater = new UpdaterBuilder("BOT_TOKEN")

    .StepOne()

    .StepTwo(inherit: false) // Add default exception handler

    .StepThree( // Quick handler
        async container => await container.Response("Started!"),
        FilterCutify.OnCommand("start"));

// ---------- Start! ----------

await updater.StartAsync(); // 🔥 Fire up and block!
```

Of course this can be even less, but these're required for production! For instance `StepTwo` adds a default exception handler ( Take a look at methods docs! )

### IHost apps

It maybe better if you use `Updater` in a app where `IServiceCollection` and `IServiceProvider` are available. Like WorkerService!

Use [this package](https://www.nuget.org/packages/TelegramUpdater.Hosting/1.0.1) for a set of useful extensions for IHosting apps.

Take a look at two examples of worker services

- [WorkerService](https://github.com/TelegramUpdater/TelegramUpdater/tree/master/Examples/WorkerService), A worker service with default updater service.
- [ManualWriterWorkerService](https://github.com/TelegramUpdater/TelegramUpdater/tree/master/Examples/ManualWriterWorker), A worker service that gets updates for a custom external service. In this example i used [PollingExtension](https://github.com/TelegramBots/Telegram.Bot.Extensions.Polling) as an external updater writer.

A quick worker service

```csharp
using WorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTelegramUpdater(
            "BOT_TOKEN",
            default,
            (builder) => builder
                .AddMessageHandler<SimpleMessageHandler>()
        );
    })
    .Build();

await host.RunAsync();
```

### Webhook app ( Asp .NET )

Webhook app is similar to IHosting app where Update Writer is external! ( Updates are written to updater from webhook Controller )

Use this [nuget package](https://www.nuget.org/packages/TelegramUpdater.Asp/1.0.1) for a full set of extensions for webhook aps.

And take a look at this [webhook example](https://github.com/TelegramUpdater/TelegramUpdater/tree/master/Examples/WebhookApp).

## Road Map

- [ ] Add ready to use handlers for all updates
- [ ] Tests
- [ ] Add more filters

## Next?!

Find documents under https://telegramupdater.github.io/Docs/ ( Yet Working on it ... )
