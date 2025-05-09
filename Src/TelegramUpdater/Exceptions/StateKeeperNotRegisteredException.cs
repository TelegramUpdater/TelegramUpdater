namespace TelegramUpdater.Exceptions;

/// <summary>
/// No state keeper registered by this name.
/// </summary>
public sealed class StateKeeperNotRegisteredException(string? stateKeeperName)
    : Exception($"There's no state keeper registered with this name ( {stateKeeperName} )")
{
}
