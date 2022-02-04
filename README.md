## Here is **Updater**

This is your telegram updater package written in C# and

The updater is supposed to fetch and handle new updates coming from bot api server

The updater is written on top of [GitHub - TelegramBots/Telegram.Bot: .NET Client for Telegram Bot API](https://github.com/TelegramBots/Telegram.Bot) package

### How it works ?

Updater uses handlers to handle you updates, and provides a lot of user friendly ( extension ) Methods to make you feel better when handling updates and writing your bot.

### Get Started

1. **Installation** 
   
   Unfortunately  there's no package  available for updater and you have to clone this repo and add references to it.

2. **Create `Updater` instance**

```csharp
var updater = new Updater(
    new TelegramBotClient("BOT_TOKEN"),
    maxDegreeOfParallelism: 10,  // maximum update process tasks count at the same time
                                 // Eg: first 10 updates are answers quickly, but others should wait
                                 // for any of that 10 to be done.

    perUserOneByOneProcess: true // a user should finish a request to go to next one.
);
```

There are 3 arguments that are explained as comments .

3. **Create update handler**

        To create an handler for a command like `/start` , you need a `MessageHandler`

        Here's  a quick message handler example:

```csharp
var myStartHandler = new MessageHandler(
    async container => await container.Response($"Next one!"),
    FilterCutify.OnCommand("start"));
```

1st argument of `MessageHandler` constructor is an asynchronous callback method, which takes an `UpdateContainerAbs<Message>` as argument.

> `UpdateContainerAbs<T>` class provides all required information to handle an update and a batch of useful Extension Methods like `Respons`, Which is an alternative for `SendTextMessageAsync` . 

2nd argument is a `Filter<Message>`

You need to specify filters to tell the `Updater` what kind of updates you except for this hander.

> `FilterCutify` is an static class that contains a lot of useful `Filter`s.

> `OnCommand` Creates an instance of `CommandFilter`, in this case like `/start`

4. Add handler to `Updater`

```csharp
updater.AddUpdateHandler(myStartHandler);
```

5. 🔥

```csharp
await updater.Start();
```

`Start` is a blocking method!