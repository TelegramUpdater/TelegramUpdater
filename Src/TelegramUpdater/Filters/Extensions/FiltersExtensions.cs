using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.Filters.Extensions;

/// <summary>
/// A set of extension methods for <see cref="IFilter{T}"/>.
/// </summary>
public static class FiltersExtensions
{
    internal static UpdaterFilterInputs<T> GetFilterInputs<T>(
        this IContainer<T> container) where T: class
    {
        return new UpdaterFilterInputs<T>(
            container.Updater,
            container.Update,
            container.Input.ScopeId,
            container.Input.LayerInfo,
            container.Input.Group,
            container.Input.Index);
    }

    internal static UpdaterFilterInputs<T> GetFilterInputs<T>(
        this IContainer container, T newBase)
    {
        return new UpdaterFilterInputs<T>(
            container.Updater,
            newBase,
            container.Input.ScopeId,
            container.Input.LayerInfo,
            container.Input.Group,
            container.Input.Index);
    }
}
