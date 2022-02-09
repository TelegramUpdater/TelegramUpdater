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
    "2015323878:AAEBfa-pTNt4fC9O1_Gw3FD9ZnreySiWhc8")

    .StepOne()

    .StepTwo(inherit: false) // Add default exception handler

    .StepThree( // Quick handler
        async container => await container.Response("Started!"),
        FilterCutify.OnCommand("start"));

// ---------- Start! ----------

await updater.Start(true); // 🔥 Fire up and block!
```

Of course this can be even less, but these're required for production! For instance `StepTwo` adds a default exception handler ( Take a look at methods docs! )

Find documents under https://telegramupdater.github.io/
