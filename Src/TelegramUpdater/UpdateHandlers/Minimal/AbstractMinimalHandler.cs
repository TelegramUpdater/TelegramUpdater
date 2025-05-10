using Microsoft.Extensions.DependencyInjection;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Singleton;

namespace TelegramUpdater.UpdateHandlers.Minimal;

internal abstract class AbstractMinimalHandler<T, TContainer, Inputs>(
    UpdateType updateType,
    Func<TContainer, Inputs, CancellationToken, Task> callback,
    IFilter<UpdaterFilterInputs<T>>? filters = default,
    Func<Update, T?>? innerUpdateResolver = default,
    bool endpoint = true) : AbstractSingletonUpdateHandler<T, TContainer>(
        updateType, innerUpdateResolver, filters, endpoint)
    where TContainer : IContainer<T> where T : class where Inputs : notnull
{
    public Task Handle(
        TContainer container,
        Inputs inputs,
        CancellationToken cancellationToken = default) => callback(container, inputs, cancellationToken);

    protected virtual Inputs ResolveInputs(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<Inputs>();
    }

    public Task HandleAsync(
        HandlerInput input,
        IServiceScope? scope = null,
        CancellationToken cancellationToken = default)
    {
        if (scope?.ServiceProvider is null)
            throw new ArgumentNullException(nameof(scope), "ActionHandlers require a service provider.");

        return Handle(ContainerBuilder(input), ResolveInputs(scope.ServiceProvider), cancellationToken);
    }
}
