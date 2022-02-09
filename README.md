# Here is **Updater**

This is your telegram updater package written in C# and

The updater is supposed to fetch and handle new updates coming from bot api server

The updater is written on top of [TelegramBots/Telegram.Bot: .NET Client for Telegram Bot API](https://github.com/TelegramBots/Telegram.Bot) package

## Getting Started

Here are starting pack for common SDKs in .NET

> TelegramUpdater in available in [nuget](https://www.nuget.org/packages/TelegramUpdater/), Install it first.

### Basic

If you're using a console app with no Hosting and `IServiceCollection` then it's your choise
**And even if you don't, you're suggested to!**

Base class of this package is `Updater`, but there's a helper class in case of basic apps called `UpdaterBuilder` which helps you
get familliar with the package.

`UpdaterBuilder` helps you build `Updater` in steps with fully documented methods.
See [UpdaterProduction](https://github.com/TelegramUpdater/TelegramUpdater/tree/master/Examples/UpdaterProduction) and
[ConsoleApp](https://github.com/TelegramUpdater/TelegramUpdater/tree/master/Examples/ConsoleApp) for instance.

If you're looking for a quick basic example:

```csharp
// See https://aka.ms/new-console-template for more information
using TelegramUpdater;

var updater = new UpdaterBuilder(
    "BOT_TOKEN")

    .StepOne()

    .StepTwo(inherit: false) // Add default exception handler

    .StepThree( // Quick handler
        async container => await container.Response("Started!"),
        FilterCutify.OnCommand("start"));

// ---------- Start! ----------

await updater.Start(true); // 🔥 Fire up and block!
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

## Next?!

Find documents under https://telegramupdater.github.io/ ( Yet Working on it ... )
