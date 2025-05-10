using Microsoft.Extensions.DependencyInjection;
using TelegramUpdater.Helpers;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.UpdateHandlers.Actions;

internal abstract class ActionHandler<T, TContainer, Inputs>(
    IServiceProvider serviceProvider,
    UpdateTypeFlags allowedUpdateTypes,
    Func<IContainer<T>, Inputs, Task> callback,
    IFilter<UpdaterFilterInputs<T>>? filters = default,
    bool endpoint = true): IUpdateHandler where TContainer: IContainer<T> where T : class where Inputs: notnull
{
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public IFilter<UpdaterFilterInputs<T>>? Filters { get; } = filters;

    public bool Endpoint { get; } = endpoint;

    public UpdateTypeFlags AllowedUpdateTypes => allowedUpdateTypes;

    public Task Handle(IContainer<T> container, Inputs inputs) => callback(container, inputs);

    protected virtual Inputs ResolveInputs()
    {
        return ServiceProvider.GetRequiredService<Inputs>();
    }

    public Task HandleAsync(HandlerInput input)
    {
        var container = new DefaultContainer<T>(
            update => update.GetInnerUpdate<T>(), input, Filters?.ExtraData);

        return Handle(container, ResolveInputs());
    }
}

internal abstract class ActionHandler<T, TContainer, In1, In2>(
    IServiceProvider serviceProvider,
    UpdateTypeFlags allowedUpdateTypes,
    Func<IContainer<T>, In1, In2, Task> callback,
    IFilter<UpdaterFilterInputs<T>>? filters = default,
    bool endpoint = true) : ActionHandler<T, TContainer, Tuple<In1, In2>>(
        serviceProvider,
        allowedUpdateTypes,
        (c, i) => callback(c, i.Item1, i.Item2),
        filters,
        endpoint),
    IUpdateHandler where TContainer : IContainer<T> where T : class where In1: notnull where In2: notnull
{
    protected override Tuple<In1, In2> ResolveInputs()
    {
        var in1 = ServiceProvider.GetRequiredService<In1>();
        var in2 = ServiceProvider.GetRequiredService<In2>();
        return new Tuple<In1, In2>(in1, in2);
    }
}

//internal abstract class ActionHandler<T, In1> where T : class
//{
//    public abstract Task Handle(IContainer<T> container, In1 input);
//}

//internal abstract class ActionHandler<T, In1, In2> where T : class
//{
//    public abstract Task Handle(IContainer<T> container, In1 first, In2 second);
//}

/// <summary>
/// A set of static extension methods for <see cref="ActionHandler{T, TContainer, Inputs}"/>s.
/// </summary>
public static class ActionHandlersExtensions
{
    
}
