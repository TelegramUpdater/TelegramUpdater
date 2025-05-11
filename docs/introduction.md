# Introduction

TelegramUpdater is feature-rich framework library for building telegram bots in c# (supports netstandard2.1 & net8.0).

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
