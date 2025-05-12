# Filters

Filters are assets you use to tell the updater which handler to use of which update. (And may be many other use cases)

Filters can combine or negated.

- And filter: filter1 & filter2
- Or filter: filter1 | filter2
- Negated filter: ~filter1

Filters can also carry data that they may acquire while checking the inputs. This data are available inside your handler or container.
For example `CommandFilter` extracts command args and store them with `args` key, and then inside of the handler you can have access to it, using something like this:

```csharp
if (container.TryGetExtraData("args", out string[]? args))
{
    // Do something ...
}

```

Or you using extension method (read magic method):

```csharp
if (container.TryParseCommandArgs(out string? arg1, out long? arg2))
{
    // You don't need to do something ...
}

```

Or for `RegexFilter` there's a `matches` key became available:

```csharp
[Regex(@"^rating_(?<rate>\d{1})$")]
[HandlerCallback(UpdateType.CallbackQuery)]
public static async Task RateAnswerHandler(IContainer<CallbackQuery> container)
{
    if (container.TryGetMatchCollection(out var matches))
    {
        var rate = matches[0].Groups["rate"].Value;
        await container.Edit($"Thank you! Your rating was {rate}.");
        await container.Answer();
    }
}
```
# Filters  

Filters are tools used to determine which handler should process a specific update. They can also serve other use cases, such as data extraction or conditional logic.  

Filters can be combined or negated to create complex conditions:  

- **And filter**: `filter1 & filter2`  
- **Or filter**: `filter1 | filter2`  
- **Negated filter**: `~filter1`  

## Data Extraction with Filters  

Filters can extract and store data while processing inputs. This data becomes accessible within your handler or container.  

### Example: CommandFilter  

The `CommandFilter` extracts command arguments and stores them under the `args` key. You can access these arguments in your handler as follows:
Alternatively, you can use an extension method for a more streamlined approach:

### Example: RegexFilter  

The `RegexFilter` makes a `matches` key available, which contains the results of the regular expression match.

## Summary  

Filters are versatile assets that not only determine handler logic but also enable data extraction and processing. By combining, negating, or extending filters, you can create powerful and flexible update-handling mechanisms.
