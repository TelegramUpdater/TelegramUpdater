namespace TelegramUpdater.Exceptions
{
    public sealed class StateKeeperNotRegistried : Exception
    {
        public StateKeeperNotRegistried(string? stateKeeperName)
            : base($"There's not state keeper registried with this name ( {stateKeeperName} )")
        {
        }
    }
}
