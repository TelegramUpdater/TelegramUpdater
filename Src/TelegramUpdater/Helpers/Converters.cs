namespace TelegramUpdater.Helpers;


/// <summary>
/// Safe convert something to something else.
/// </summary>
/// <typeparam name="T"></typeparam>
internal interface IConvertTo<T>
{
    public T Convert();
}

/// <summary>
/// Error prone convert something to something else.
/// </summary>
/// <typeparam name="T"></typeparam>
internal interface IMayConvertTo<T>
{
    public T? Convert();
}

