namespace TelegramUpdater.UpdateHandlers.Scoped;

/// <summary>
/// Base interface for scoped update handlers.
/// </summary>
public interface IScopedUpdateHandler : IUpdateHandler
{
    internal IReadOnlyDictionary<string, object>? ExtraData { get; }

    internal void SetExtraData(
        IReadOnlyDictionary<string, object>? extraData);
}
