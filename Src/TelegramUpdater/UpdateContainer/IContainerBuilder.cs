namespace TelegramUpdater.UpdateContainer;

/// <summary>
/// This interface indicates how a container should be built
/// </summary>
public interface IContainerBuilder<T, TContainer>
    where TContainer : IContainer<T>
    where T: class
{
    /// <summary>
    /// Create the container using input <see cref="HandlerInput"/>.
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public TContainer CreateContainer(HandlerInput inputs);
}
