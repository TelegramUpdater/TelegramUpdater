using TelegramUpdater.RainbowUtlities;

namespace TelegramUpdater
{
    /// <summary>
    /// Abstract class to build a pre update processor.
    /// </summary>
    public abstract class AbstractPreUpdateProcessor
    {
        /// <summary>
        /// If you're using a <see cref="IServiceProvider"/>, use this ctor and get
        /// <see cref="IUpdater"/> from services.
        /// </summary>
        /// <param name="updater">The updater.</param>
        protected AbstractPreUpdateProcessor(IUpdater updater)
        {
            Updater = updater;
        }

        /// <summary>
        /// An empty ctor in case of no service provider.
        /// </summary>
        protected AbstractPreUpdateProcessor()
        {
        }

        internal void SetUpdater(IUpdater updater)
        {
            Updater = updater;
        }

        /// <summary>
        /// Updater instance.
        /// </summary>
        protected IUpdater Updater { get; private set; } = null!;

        /// <summary>
        /// A function that will be called before any update process.
        /// </summary>
        /// <param name="shiningInfo">Shining info related to that update.</param>
        /// <returns>Returns <see langword="false"/> to stop processing this update,
        /// otherwise <see langword="true"/></returns>.
        public abstract Task<bool> PreProcessor(ShiningInfo<long, Update> shiningInfo);
    }
}
