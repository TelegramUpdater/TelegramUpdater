namespace TelegramUpdater.Filters
{
    /// <summary>
    /// Select the mode of arguments fro command handler
    /// </summary>
    public enum ArgumentsMode
    {
        /// <summary>
        /// If your command filter should have arguments
        /// </summary>
        Require,

        /// <summary>
        /// If your command filter should not have arguments
        /// </summary>
        NoArgs,

        /// <summary>
        /// If you don't care about your command filter arguments
        /// </summary>
        Idc
    }
}
