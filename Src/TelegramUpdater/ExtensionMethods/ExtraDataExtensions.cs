using System.Diagnostics.CodeAnalysis;
using TelegramUpdater.Filters;
using TelegramUpdater.Helpers;

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

            data = default;
            return false;
        }

        /// <summary>
        /// Tries to get and parse the command args. you should have a
        /// <see cref="CommandFilter"/>..
        /// </summary>
        public static bool TryParseCommandArgs<T, T1>(
            this IContainer<T> container,
            [NotNullWhen(true)] out T1? output1) where T : class
        {
            if (container.TryGetExtraData("args", out string[]? commandArgs))
            {
                if (commandArgs.TryConvertArgs(out output1))
                {
                    return true;
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "Command args not found. do you have any CommandFilters there?"
                );
            }

            output1 = default;
            return false;
        }

        /// <summary>
        /// Tries to get and parse the command args. you should have a
        /// <see cref="CommandFilter"/>..
        /// </summary>
        public static bool TryParseCommandArgs<T, T1, T2>(
            this IContainer<T> container,
            [NotNullWhen(true)] out T1? output1,
            [NotNullWhen(true)] out T2? output2) where T : class
        {
            if (container.TryGetExtraData("args", out string[]? commandArgs))
            {
                if (commandArgs.TryConvertArgs(out output1, out output2))
                {
                    return true;
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "Command args not found. do you have any CommandFilters there?"
                );
            }

            output1 = default;
            output2 = default;
            return false;
        }

        /// <summary>
        /// Tries to get and parse the command args. you should have a
        /// <see cref="CommandFilter"/>..
        /// </summary>
        public static bool TryParseCommandArgs<T, T1, T2, T3>(
            this IContainer<T> container,
            [NotNullWhen(true)] out T1? output1,
            [NotNullWhen(true)] out T2? output2,
            [NotNullWhen(true)] out T3? output3) where T : class
        {
            if (container.TryGetExtraData("args", out string[]? commandArgs))
            {
                if (commandArgs.TryConvertArgs(out output1, out output2, out output3))
                {
                    return true;
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "Command args not found. do you have any CommandFilters there?"
                );
            }

            output1 = default;
            output2 = default;
            output3 = default;
            return false;
        }

        /// <summary>
        /// Tries to get and parse the command args. you should have a
        /// <see cref="CommandFilter"/>..
        /// </summary>
        public static bool TryParseCommandArgs<T, T1, T2, T3, T4>(
            this IContainer<T> container,
            [NotNullWhen(true)] out T1? output1,
            [NotNullWhen(true)] out T2? output2,
            [NotNullWhen(true)] out T3? output3,
            [NotNullWhen(true)] out T4? output4) where T : class
        {
            if (container.TryGetExtraData("args", out string[]? commandArgs))
            {
                if (commandArgs.TryConvertArgs(out output1, out output2, out output3, out output4))
                {
                    return true;
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "Command args not found. do you have any CommandFilters there?"
                );
            }

            output1 = default;
            output2 = default;
            output3 = default;
            output4 = default;
            return false;
        }

    }
}
