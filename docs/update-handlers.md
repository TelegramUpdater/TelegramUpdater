# Update handlers

One the most important part of TelegramUpdater are update handlers. Update handlers
consist of two main parts (and lots of other parts).

- Filters
- Callbacks

If a handlers's filter is passed then the callback method is called that
runs your handling logic.

Most handler (all that you may use) are generic over type of inner update. Inner update
or actual update is a word you may see or hear a lot in this package, it refers to
one of type `Update` properties. As instance for `Update.Message`, inner update is `Message`.

> [!NOTE]
> Some inner update properties share the same type. For example, `Update.Message`,
> `Update.EditedMessage`, `Update.ChannelPost` and etc all are of type `Message`.

Other thing that an update handler is generic over, is `TContainer` or the type of the container.
If you see an update handler that is not generic over this, it means the container type is something like: `DefaultContainer`.
Most times you can simply use `IContainer<T>` which covers any container.
Containers are (mostly) all you have to handle your updates. It's made of `IUpdater`, `Update`, `ActualUpdate` and etc.
This makes containers a very good type to make extension methods for.

Containers are also generic over inner update types.

> [!NOTE]
> Type `Update` itself doesn't contain any useful information (at least for you, or even me). It got just an UpdateId.

Filters are also generic over inner updates.

As you can see this is a super generic and absolute package. When you're creating a handler, you can and must choose
inner update type and container type. Filter you're using should also match the handler update type.

> [!NOTE]
> Most times we made things easier to set container type to default and don't border you deciding it.

> [!NOTE]
> Some filters are shared between many handler types. Eg: RegexFilter will work on message text or caption or callback query's
> data and etc. You have to make sure you're using the right one.
>
> If you're using filters as attributes, the package will take care of choosing the right variant of the filter, and
> throws an exception if there's non matching the update type.

## Handler kinds

There are currently 4 kind of update handlers available to use (sorted by advancement):

- **Singleton (normal) handlers**

These are the most straight forward handler that mostly just need a callback and optionally (I say surly) a filter.
you'll get a container inside callback to handle the stuff.

You can use them like:

```csharp

.Handle(
    UpdateType.Message,
    async (MessageContainer container) =>
    {
        await container.Response("Want me to help you?!");
    },
    ReadyFilters.OnCommand("help"))

```

> [!NOTE]
> Registering message handlers, force you specify UpdateType as there are a lot of type of message update (as I mentioned before).
> ChatMember update are also require UpdateType.

> [!WARNING]
> From now on, all other type of handlers are depend on service providers. So in other to use them, your updater
> should be initialized using a host provider like the one in ASP .NET or worker services or your custom one.

- **Minimal (singleton, service availability) handlers**

This type of handlers are just like singleton handlers (as they are), but they can provide access to services
inside your callback function parameters beside container.

```csharp

.Handle(
    UpdateType.Message,
    async (IContainer<Message> container, PlaygroundMemory memory) =>
    {
        var records = await memory.SeenUsers.CountAsync();
        await container.Response($"I've seen {records} people so far.");
    },
    ReadyFilters.OnCommand("records") & ReadyFilters.PM())

```

As you see they are very similar to normal handlers (Except in parameters).

- **Scoped (service availability) handlers**

If you need a extensive use of services and you handler is becoming big, you
can switch to scoped handlers. They are c# classes that you define and you can use any
service available in you DI inside their constructor (Just like how DI works).

```csharp

using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

// ~~~ sniff ~~~

[Regex(@"^rating_(?<rate>\d{1})$")]
internal class RateAnswer(IFancyService service) : CallbackQueryHandler
{
    protected override async Task HandleAsync(CallbackQueryContainer container)
    {
        if (container.TryGetMatchCollection(out var matches))
        {
            var rate = matches[0].Groups["rate"].Value;
            await Edit($"Thank you! Your rating was {rate}.");
            await Answer();
        }
    }
}

```

And then you add them to the updater.

```csharp

updaterSeriviceBuilder
    .AddMessageHandler<RateAnswer>()

```

If you place your handlers into right places, Updater can collect them for you by calling
`updaterSeriviceBuilder.CollectHandlers()`.

> [!NOTE]
> The default right place for handlers is `UpdateHandlers/Messages` for message handlers
> and other type of handlers accordingly.

- **Controller (service availability) handlers**

These are just like scoped handlers, but they are not restricted to single
`HandleAsync` method, you can add several callback methods (named actions)
mark them with `HandlerAction` attribute and use different filters on them by attributes.

Controller handlers enable you to use less filter resolve most services once per different callback and more.

```csharp
[Command("about")]
internal class About : MessageControllerHandler
{
    [HandlerAction]
    [ChatType(ChatTypeFlags.Private)]
    public async Task Private([ResolveService] PlaygroundMemory memory)
    {
        var seenUsers = await memory.SeenUsers.CountAsync();
        await Response($"This's what you need to know about me: I've seen {seenUsers} people so far.");
    }

    [HandlerAction]
    [ChatType(ChatTypeFlags.Group | ChatTypeFlags.SuperGroup)]
    public async Task Group(IContainer<Message> container)
    {
        var aboutDeeplink = await Updater.CreateDeeplink("about");
        var aboutButton = InlineKeyboardButton.WithUrl("Continue in private", aboutDeeplink);
        await container.Response(
            "Theses are private talks!",
            replyMarkup: new InlineKeyboardMarkup(aboutButton));
    }
}
```

They can be added to the updater exactly like scoped handlers (as they are).
And just like scoped handlers you can have access to services from class
constructor to be available inside all actions.

## Other things to know

There are some other concepts about handlers that worth knowing.

1. Group (`integer`)

    Handler group defines its order of handling. A group with lower group will be handled sooner.

    This can be set using attribute `[ScopedHandler]` (for scoped handlers), or by setting `HandlingOptions` when adding a handler in `Handle` method.

2. Endpoint (`bool`)

    This indicates if this is the end of handling pipeline for an incoming update or not. By default all handlers are endpoint, meaning if their got passed, no other handler (in the layer) will be checked for this update.

    If you set `Endpoint = false`, the updater will continue handling pipeline and proceed to a higher `Group` handler.

    This can be set by overriding it (for scoped handlers), or when adding a handler in `Handle` method.

3. Layer (LayerGroup `integer`)

    Handlers can effect handling process by for example breaking it or continuing.
    For example `Endpoint = true` handlers break the handling process.

    But this breaking, only effects handler in a same layer.

    The handlers will first group by their layer and then order by `LayerGroup`. In each layer, the handlers are ordered by their own `Group`.

    Beside `Endpoint` property, there are two special exceptions that effect handling pipeline.

    - `ContinuePropagation`: Stops the handler an proceed to the next handler (For now its just like early returning).

    - `BreakPropagation`: This will break handling pipeline for a layer and proceeds to the next layer.

4. Scope (ScopeId `Guid`)

    Handling scope refers to "from when and update comes to where all handling process finishes for it".

    Each scope will have a `Guid` as scope id that is available in container to check.

    When you see an extension method like `container.AddScopedItem(...)` mean the item is only available
    in this handling update for this update. When the scoped finishes, the items will expire.

    The above statement is also correct for layers. A Layer item is only valid a handlers in a layer with an specific scope id.

Using `ScopeId` or `LayerId`, helps you and the updater to get raid of temporary data added to the updater as soon as possible and reduce data collisions. When you add a scope data, you're ensured that the data is only valid for handlers after this handler and for the same incoming update. (See [Seen.cs](https://github.com/TelegramUpdater/TelegramUpdater/blob/master/Examples/Playground/UpdateHandlers/Messages/Status.cs) from Playground)
