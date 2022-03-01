using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.UpdateContainer
{
    /// <summary>
    /// Extension method related to extra data.
    /// </summary>
    public static class ExtraDataExtensions
    {
        /// <summary>
        /// Tries to fetch a data from container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="key">The key to data.</param>
        /// <param name="data">The data itself</param>
        /// <returns></returns>
        public static bool TryGetExtraData<T>(
            this IContainer<T> container,
            string key,
            [NotNullWhen(true)] out object? data) where T : class
        {
            if (container.ContainsKey(key))
            {
                data = container[key];
                return data is not null;
            }
            data = null;
            return false;
        }

        /// <summary>
        /// Tries to get and cast a data to <typeparamref name="K"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool TryGetExtraData<T, K>(
            this IContainer<T> container,
            string key,
            [NotNullWhen(true)] out K? data) where T : class
        {
            if (container.TryGetExtraData(key, out object? innerData))
            {
                try
                {
                    data = (K)innerData;
                    return true;
                }
                catch (InvalidCastException)
                {
                    data = default;
                    return false;
                }
            }
            else
            {
                data = default;
                return false;
            }
        }
    }
}
