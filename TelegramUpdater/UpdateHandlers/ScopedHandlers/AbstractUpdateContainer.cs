using System.Reflection;
using TelegramUpdater.FilterAttributes;

namespace TelegramUpdater.UpdateHandlers.ScopedHandlers
{
    /// <summary>
    /// Abstract base for <see cref="IScopedUpdateHandler"/> containers.
    /// </summary>
    /// <typeparam name="THandler">
    /// The handler, which is <see cref="IScopedUpdateHandler"/>
    /// </typeparam>
    /// <typeparam name="TUpdate">Update type.</typeparam>
    public abstract class AbstractUpdateContainer<THandler, TUpdate>
        : IScopedHandlerContainer where THandler
        : IScopedUpdateHandler where TUpdate : class
    {
        private readonly IFilter<TUpdate>? _filter;

        internal AbstractUpdateContainer(
            UpdateType updateType, IFilter<TUpdate>? filter = default)
        {
            if (updateType == UpdateType.Unknown)
                throw new ArgumentException(
                    $"There's nothing unknown here! {nameof(updateType)}");

            UpdateType = updateType;
            ScopedHandlerType = typeof(THandler);

            _filter = filter;

            if (_filter == null)
            {
                // Check for attributes
                _filter = GetFilterAttributes<TUpdate>(ScopedHandlerType);
            }
        }

        /// <inheritdoc/>
        public Type ScopedHandlerType { get; }

        /// <inheritdoc/>
        public UpdateType UpdateType { get; }

        IReadOnlyDictionary<string, object>? IScopedHandlerContainer.ExtraData
            => _filter?.ExtraData;

        /// <summary>
        /// Checks if an update can be handled in a handler of type <see cref="ScopedHandlerType"/>.
        /// </summary>
        /// <param name="t">The inner update.</param>
        /// <returns></returns>
        private bool ShouldHandle(TUpdate t)
        {
            if (_filter is null) return true;

            return _filter.TheyShellPass(t);
        }

        /// <summary>
        /// A function to extract actual update from <see cref="Update"/>.
        /// </summary>
        /// <param name="update">The update.</param>
        /// <returns></returns>
        internal protected abstract TUpdate? GetT(Update update);

        /// <inheritdoc/>
        bool IScopedHandlerContainer.ShouldHandle(Update update)
        {
            if (update.Type != UpdateType) return false;

            var insider = GetT(update);

            if (insider == null) return false;

            return ShouldHandle(insider);
        }

        internal static IFilter<T> GetFilterAttributes<T>(Type type)
            where T : class
        {
            Filter<T> filter = null!;
            var applied = type.GetCustomAttributes<AbstractFilterAttribute>();
            foreach (var item in applied)
            {
                var f = (Filter<T>?)item.GetFilterTypeOf(typeof(T));
                if (f != null)
                {
                    if (item.Reverse)
                    {
                        f = ~f; // Reverse the filter.
                    }

                    if (filter == null)
                    {
                        filter = f;
                    }
                    else
                    {
                        if (item.ApplyAsOr)
                        {
                            filter |= f;
                        }
                        else
                        {
                            filter &= f;
                        }
                    }
                }
            }

            return filter;
        }
    }
}
