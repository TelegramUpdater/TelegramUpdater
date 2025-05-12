using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Controller.Attributes;
using TelegramUpdater.UpdateHandlers.Scoped;

namespace TelegramUpdater.UpdateHandlers.Controller;

/// <summary>
/// These are actually <see cref="IScopedUpdateHandler"/>s, but they can have
/// multiple handler functions inside them (called actions).
/// <para>
/// This enables the handlers to have overall filters for a group or chain of handler and use less filters and 
/// also this helps reusing services and more.
/// </para>
/// </summary>
public abstract class AbstractControllerUpdateHandler<T, TContainer>(Func<Update, T?>? getT)
    : AbstractScopedUpdateHandler<T, TContainer>(getT) where T : class where TContainer : IContainer<T>
{
    // Overriding this to don't allow users to do so in controller handlers.
    /// <inheritdoc/>
    protected override Task HandleAsync()
    {
        return Task.CompletedTask;
    }

    // Overriding this to don't allow users to do so in controller handlers.
    /// <inheritdoc/>
    protected override Task HandleAsync(TContainer container)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override async Task HandleAsync(TContainer container, IServiceScope? scope = null, CancellationToken cancellationToken = default)
    {
        // Get all methods in the current class marked with HandlerActionAttribute
        var methods = GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Select(m => new { Info = m, Options = m.GetCustomAttribute<HandlerActionAttribute>() })
            .Where(m => m.Options is not null)
            .OrderBy(m => m.Options!.Group);

        foreach (var m in methods)
        {
            var method = m.Info;
            var options = m.Options!;

            // Check if the method has any filters and evaluate them
            var filters = method.GetFilterAttributes<UpdaterFilterInputs<T>>();

            if (filters is not null && !filters.TheyShellPass(
                new UpdaterFilterInputs<T>(
                    container.Updater,
                    container.Update,
                    container.Input.ScopeId,
                    container.Input.LayerInfo,
                    container.Input.Group,
                    container.Input.Index)))
            {
                continue; // Skip this method if any filter fails
            }

            // Resolve method parameters
            var parameters = method.GetParameters();
            var arguments = new object?[parameters.Length];
            var ignore = false;

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (parameter.GetCustomAttribute<ResolveServiceAttribute>() is ResolveServiceAttribute resolve)
                {
                    // Resolve service from the scope
                    if (resolve.Required && scope == null)
                    {
                        if (options.IgnoreIfParametersNotFound)
                        {
                            ignore = true;
                            break;
                        }

                        throw new InvalidOperationException("Service scope is required to resolve dependencies.");
                    }

                    var resolved = scope?.ServiceProvider.GetService(parameter.ParameterType);
                    if (resolved == null && resolve.Required)
                    {
                        if (options.IgnoreIfParametersNotFound)
                        {
                            ignore = true;
                            break;
                        }

                        throw new InvalidOperationException($"Unable to resolve parameter '{parameter.Name}' of type '{parameter.ParameterType}' which is required.");
                    }

                    arguments[i] = resolved;
                }
                else if (parameter.GetCustomAttribute<ExtraDataAttribute>(inherit: true) is ExtraDataAttribute extraData)
                {
                    if (extraData.TryFind(parameter.ParameterType, filters?.ExtraData, ExtraData, out var found))
                    {
                        arguments[i] = found;
                    }
                    else
                    {
                        if (options.IgnoreIfParametersNotFound)
                        {
                            ignore = true;
                            break;
                        }

                        throw new InvalidOperationException(
                            "Unable to resolve parameter '{parameter.Name}' of type '{parameter.ParameterType}' from extra data.");
                    }
                }
                else if (parameter.ParameterType.IsAssignableFrom(container.GetType()))
                {
                    arguments[i] = container;
                }
                else
                {
                    if (options.IgnoreIfParametersNotFound)
                    {
                        ignore = true;
                        break;
                    }

                    throw new InvalidOperationException($"Unable to resolve parameter '{parameter.Name}' of type '{parameter.ParameterType}'.");
                }
            }

            // Invoke the method
            if (!ignore)
                if (method.Invoke(this, arguments) is Task task)
                {
                    // TODO: Should we keep ignoring exceptions here?
                    // Currently the exceptions will go back to ExceptionHandler.
                    // And will stop propagation to other actions.
                    await task.ConfigureAwait(false);
                    if (options.Endpoint)
                    {
                        break;
                    }
                }
        }
    }
}
