using System;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramUpdater.RainbowUtlities
{
    public static class RainbowExtensions
    {
        /// <summary>
        /// Starts shining ( non blocking ).
        /// </summary>
        /// <param name="callback">A callback function to call for each object.</param>
        /// <returns></returns>
        public static void Shine<TId, TValue>(
            this Rainbow<TId, TValue> rainbow,
            Func<ShiningInfo<TId, TValue>, CancellationToken, Task> callback,
            Func<Exception, CancellationToken, Task>? exceptionHandler = default,
            CancellationToken cancellationToken = default) where TId : struct
        {
            Task.Run(async () =>
            {
                try
                {
                    await rainbow.ShineAsync(callback, exceptionHandler, cancellationToken);
                }
                catch (Exception ex)
                {
                    if (exceptionHandler != null)
                    {
                        try
                        {
                            await exceptionHandler(ex, cancellationToken);
                        }
                        catch { }
                    }
                }

            }, cancellationToken);
        }
    }
}
