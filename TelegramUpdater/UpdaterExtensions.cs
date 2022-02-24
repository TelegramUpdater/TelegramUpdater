namespace TelegramUpdater
{
    public static class UpdaterExtensions
    {
        public static object GetInnerUpdate(this Update update)
        {
            if (update.Type == UpdateType.Unknown)
                throw new ArgumentException($"Can't resolve Update of Type {update.Type}");

            return typeof(Update).GetProperty(update.Type.ToString())?.GetValue(update, null)
                ?? throw new InvalidOperationException($"Inner update is null for {update.Type}");
        }

        public static T GetInnerUpdate<T>(this Update update)
        {
            if (update.Type == UpdateType.Unknown)
                throw new ArgumentException($"Can't resolve Update of Type {update.Type}");

            return (T)(typeof(Update).GetProperty(update.Type.ToString())?.GetValue(update, null)
                ?? throw new InvalidOperationException($"Inner update is null for {update.Type}"));
        }

        public static UpdateType? GetUpdateType<T>()
        {
            if (Enum.TryParse(typeof(T).Name, out UpdateType result))
            {
                return result;
            }

            return null;
        }
    }
}
