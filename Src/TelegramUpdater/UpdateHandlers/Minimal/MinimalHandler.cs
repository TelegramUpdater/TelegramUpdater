using Microsoft.Extensions.DependencyInjection;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.Minimal;

internal class MinimalHandler<T, In1>(
    UpdateType updateType,
    Func<IContainer<T>, In1, CancellationToken, Task> callback,
    IFilter<UpdaterFilterInputs<T>>? filters = default,
    Func<Update, T?>? innerUpdateResolver = default,
    bool endpoint = true) : AbstractMinimalHandler<T, DefaultContainer<T>, In1>(
        updateType, callback, filters, innerUpdateResolver, endpoint)
    where T : class where In1 : notnull
{
    internal override DefaultContainer<T> ContainerBuilder(HandlerInput input)
        => new(InnerUpdateExtractor, input, Filter?.ExtraData);
}

internal class MinimalHandler<T, In1, In2>(
    UpdateType updateType,
    Func<IContainer<T>, In1, In2, CancellationToken, Task> callback,
    IFilter<UpdaterFilterInputs<T>>? filters = default,
    Func<Update, T?>? innerUpdateResolver = default,
    bool endpoint = true) : MinimalHandler<T, Tuple<In1, In2>>(
        updateType,
        (c, i, ct) => callback(c, i.Item1, i.Item2, ct),
        filters,
        innerUpdateResolver,
        endpoint)
    where T : class where In1 : notnull where In2 : notnull
{
    protected override Tuple<In1, In2> ResolveInputs(IServiceProvider serviceProvider)
    {
        var in1 = serviceProvider.GetRequiredService<In1>();
        var in2 = serviceProvider.GetRequiredService<In2>();
        return new Tuple<In1, In2>(in1, in2);
    }
}

internal class MinimalHandler<T, In1, In2, In3>(
    UpdateType updateType,
    Func<IContainer<T>, In1, In2, In3, CancellationToken, Task> callback,
    IFilter<UpdaterFilterInputs<T>>? filters = default,
    Func<Update, T?>? innerUpdateResolver = default,
    bool endpoint = true) : MinimalHandler<T, Tuple<In1, In2, In3>>(
        updateType,
        (c, i, ct) => callback(c, i.Item1, i.Item2, i.Item3, ct),
        filters,
        innerUpdateResolver,
        endpoint)
    where T : class where In1 : notnull where In2 : notnull where In3 : notnull
{
    protected override Tuple<In1, In2, In3> ResolveInputs(IServiceProvider serviceProvider)
    {
        var in1 = serviceProvider.GetRequiredService<In1>();
        var in2 = serviceProvider.GetRequiredService<In2>();
        var in3 = serviceProvider.GetRequiredService<In3>();

        return new Tuple<In1, In2, In3>(in1, in2, in3);
    }
}

internal class MinimalHandler<T, In1, In2, In3, In4>(
    UpdateType updateType,
    Func<IContainer<T>, In1, In2, In3, In4, CancellationToken, Task> callback,
    IFilter<UpdaterFilterInputs<T>>? filters = default,
    Func<Update, T?>? innerUpdateResolver = default,
    bool endpoint = true) : MinimalHandler<T, Tuple<In1, In2, In3, In4>>(
        updateType,
        (c, i, ct) => callback(c, i.Item1, i.Item2, i.Item3, i.Item4, ct),
        filters,
        innerUpdateResolver,
        endpoint)
    where T : class where In1 : notnull where In2 : notnull where In3 : notnull where In4 : notnull
{
    protected override Tuple<In1, In2, In3, In4> ResolveInputs(IServiceProvider serviceProvider)
    {
        var in1 = serviceProvider.GetRequiredService<In1>();
        var in2 = serviceProvider.GetRequiredService<In2>();
        var in3 = serviceProvider.GetRequiredService<In3>();
        var in4 = serviceProvider.GetRequiredService<In4>();

        return new Tuple<In1, In2, In3, In4>(in1, in2, in3, in4);
    }
}
