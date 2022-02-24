using System;
using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.FilterAttributes
{
    /// <summary>
    /// Using this class you can build filter attributes which can be cross update type.
    /// </summary>
    public abstract class FilterAttributeBuilder : AbstractFilterAttribute
    {
        private readonly Dictionary<Type, object> _filtersPerUpdateType;

        /// <summary>
        /// Init a new instance of <see cref="FilterAttributeBuilder"/>.
        /// </summary>
        /// <remarks>
        /// User <see cref="AddFilterForUpdate{T}(Filter{T})"/>
        /// to add filters for different type of updates.
        /// </remarks>
        public FilterAttributeBuilder(Action<FilterAttributeBuilder> builder)
        {
            _filtersPerUpdateType = new();

            builder(this);
        }

        /// <summary>
        /// Add a filter for another type of update <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="Message"/>, <see cref="CallbackQuery"/> and more ...
        /// </remarks>
        /// <typeparam name="T">Type of update.</typeparam>
        /// <param name="filter">Filter to apply on updates of type <typeparamref name="T"/>.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public FilterAttributeBuilder AddFilterForUpdate<T>(Filter<T> filter) where T : class
        {
            var t = typeof(T);
            if (!Enum.TryParse(t.Name, out UpdateType _))
            {
                throw new InvalidOperationException($"{typeof(T)} is not an update!");
            }

            _filtersPerUpdateType.Add(t, filter);
            return this;
        }

        /// <inheritdoc/>
        protected internal override object GetFilterTypeOf(Type requestedType)
        {
            if (!_filtersPerUpdateType.ContainsKey(requestedType))
            {
                throw new InvalidOperationException($"{GetType()} dose not support {requestedType}");
            }

            return _filtersPerUpdateType[requestedType];
        }
    }
}
