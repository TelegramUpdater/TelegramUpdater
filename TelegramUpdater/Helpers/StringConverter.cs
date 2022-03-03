using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.Helpers
{
    /// <summary>
    /// A static class that contains methods for converting strings.
    /// </summary>
    public static class StringConverter
    {
        /// <summary>
        /// Tries to convert a string to a given type
        /// </summary>
        /// <param name="type">Should convert to this</param>
        /// <param name="input">Input string to convert</param>
        /// <param name="output">Output result</param>
        /// <returns></returns>
        public static bool TryConvert(
            this string input,
            Type type,
            [NotNullWhen(true)] out object? output)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(type);
                if (converter != null)
                {
                    output = converter.ConvertFromString(input);
                    if (output != null)
                    {
                        return true;
                    }
                    return false;
                }

                output = default;
                return false;
            }
            catch (ArgumentException)
            {
                output = default;
                return false;
            }
            catch (FormatException)
            {
                output = default;
                return false;
            }
            catch (NotSupportedException)
            {
                output = default;
                return false;
            }
        }

        /// <summary>
        /// Tries to convert a string to a given type
        /// </summary>
        /// <typeparam name="T">Should convert to this</typeparam>
        /// <param name="input">Input string to convert</param>
        /// <param name="output">Output result</param>
        public static bool TryConvert<T>(
            this string input,
            [NotNullWhen(true)] out T? output)
        {
            if (input.TryConvert(typeof(T), out var innerOutput))
            {
                output = (T)innerOutput;
                return true;
            }

            output = default;
            return false;
        }

        /// <summary>
        /// Tries to convert an string array elements to given types
        /// </summary>
        public static bool TryConvertArgs<T1>(
            this string[] args,
            [NotNullWhen(true)] out T1? output1,
            int startIndex = 0)
        {
            if (args.Length > startIndex)
            {
                if (args[startIndex].TryConvert(out output1))
                {
                    return true;
                }
            }

            output1 = default;
            return false;
        }

        /// <summary>
        /// Tries to convert an string array elements to given types
        /// </summary>
        public static bool TryConvertArgs<T1, T2>(
            this string[] args,
            [NotNullWhen(true)] out T1? output1,
            [NotNullWhen(true)] out T2? output2,
            int startIndex = 0)
        {
            if (args.Length < startIndex + 2)
            {
                output1 = default;
                output2 = default;
                return false;
            }

            if (args[startIndex].TryConvert(out output1) &&
                args[startIndex + 1].TryConvert(out output2))
            {
                return true;
            }

            output1 = default;
            output2 = default;
            return false;
        }

        /// <summary>
        /// Tries to convert an string array elements to given types
        /// </summary>
        public static bool TryConvertArgs<T1, T2, T3>(
            this string[] args,
            [NotNullWhen(true)] out T1? output1,
            [NotNullWhen(true)] out T2? output2,
            [NotNullWhen(true)] out T3? output3,
            int startIndex = 0)
        {
            if (args.Length < startIndex + 3)
            {
                output1 = default;
                output2 = default;
                output3 = default;
                return false;
            }

            if (args[startIndex].TryConvert(out output1) &&
                args[startIndex + 1].TryConvert(out output2) &&
                args[startIndex + 2].TryConvert(out output3))
            {
                return true;
            }

            output1 = default;
            output2 = default;
            output3 = default;
            return false;
        }

        /// <summary>
        /// Tries to convert an string array elements to given types
        /// </summary>
        public static bool TryConvertArgs<T1, T2, T3, T4>(
            this string[] args,
            [NotNullWhen(true)] out T1? output1,
            [NotNullWhen(true)] out T2? output2,
            [NotNullWhen(true)] out T3? output3,
            [NotNullWhen(true)] out T4? output4,
            int startIndex = 0)
        {
            if (args.Length < startIndex + 4)
            {
                output1 = default;
                output2 = default;
                output3 = default;
                output4 = default;
                return false;
            }

            if (args[startIndex].TryConvert(out output1) &&
                args[startIndex + 1].TryConvert(out output2) &&
                args[startIndex + 2].TryConvert(out output3) &&
                args[startIndex + 3].TryConvert(out output4))
            {
                return true;
            }

            output1 = default;
            output2 = default;
            output3 = default;
            output4 = default;
            return false;
        }
    }
}
