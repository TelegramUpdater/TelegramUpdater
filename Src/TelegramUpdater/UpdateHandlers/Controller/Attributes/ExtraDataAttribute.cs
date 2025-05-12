namespace TelegramUpdater.UpdateHandlers.Controller.Attributes;

/// <summary>
/// This attribute marks a parameter that its value should be acquired from the
/// <see cref="IFilter{T}.ExtraData"/>.
/// </summary>
/// <remarks>
/// Type of the parameter and the type of the requested data should be assign able.
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class ExtraDataAttribute(string key) : Attribute
{
    /// <summary>
    /// The key of the extra data.
    /// </summary>
    public string Key { get; } = key;

    /// <summary>
    /// Indicates that the key should also be searched in controller's filter data.
    /// </summary>
    public bool IncludeController { get; set; } = true;

    /// <summary>
    /// Indicates that the key must exists.
    /// </summary>
    public bool Required { get; set; } = true;

    /// <summary>
    /// This method is used to polish the value before returning it.
    /// </summary>
    internal protected virtual bool Polish(Type type, object? input, out object? output)
    {
        output = input;
        return true;
    }

    internal bool TryFind(
        Type type,
        IReadOnlyDictionary<string, object>? actionExtraData,
        IReadOnlyDictionary<string, object>? controllerExtraData, 
        out object? output)
    {
        if (actionExtraData?.TryGetValue(Key, out var val) ?? false)
        {
            if (Polish(type, val, out var polished))
            {
                if (polished is not null && !type.IsAssignableFrom(polished.GetType()))
                {
                    output = null;
                    return false;
                }

                output = polished;
                return true;
            }
        }

        if (IncludeController &&
            (controllerExtraData?.TryGetValue(Key, out var valFromController) ?? false))
        {
            if (Polish(type, valFromController, out var polished))
            {
                if (polished is not null && !type.IsAssignableFrom(polished.GetType()))
                {
                    output = null;
                    return false;
                }

                output = polished;
                return true;
            }
        }

        if (Required)
        {
            output = null;
            return false;
        }

        output = null;
        return true;
    }
}
