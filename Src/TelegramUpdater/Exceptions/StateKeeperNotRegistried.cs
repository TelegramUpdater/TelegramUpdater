namespace TelegramUpdater.Exceptions;

public sealed class StateKeeperNotRegistried : Exception
{
    public StateKeeperNotRegistried(string? stateKeeperName)
        : base($"There's no state keeper registered with this name ( {stateKeeperName} )")
    {
    }
}
