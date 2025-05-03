using System.Runtime.InteropServices;

namespace TelegramUpdater.Helpers;

/// <summary>
/// 
/// </summary>
public class HandlingStoragesKeys
{
    /// <summary>
    /// Scope id.
    /// </summary>
    /// <param name="Id">The <see cref="Guid"/> of this scope.</param>
    public readonly record struct ScopeId(Guid Id);

    /// <summary>
    /// Layer id.
    /// </summary>
    /// <remarks>
    /// Composed of <paramref name="ScopeId"/> and <paramref name="Id"/> as layer id.
    /// </remarks>
    /// <param name="ScopeId">Scope id.</param>
    /// <param name="Id">Layer id.</param>
    [StructLayout(LayoutKind.Auto)]
    public readonly record struct LayerId(Guid ScopeId, int Id);

    /// <summary>
    /// Group id.
    /// </summary>
    /// <remarks>
    /// Composed of <paramref name="ScopeId"/> and <paramref name="LayerId"/> as layer id
    /// and <paramref name="Id"/> as group.
    /// </remarks>
    /// <param name="ScopeId">Scope id.</param>
    /// <param name="LayerId">Layer Id.</param>
    /// <param name="Id">Group.</param>
    [StructLayout(LayoutKind.Auto)]
    public readonly record struct GroupId(Guid ScopeId, int LayerId, int Id);

    /// <summary>
    /// Handler id.
    /// </summary>
    /// <remarks>
    /// Composed of <paramref name="ScopeId"/> and <paramref name="LayerId"/> as layer id
    /// and <paramref name="GroupId"/> as group and finally <paramref name="Index"/> in the group.
    /// </remarks>
    /// <param name="ScopeId">Scope id.</param>
    /// <param name="LayerId">Layer Id.</param>
    /// <param name="GroupId">Group.</param>
    /// <param name="Index">Index in group.</param>
    [StructLayout(LayoutKind.Auto)]
    public readonly record struct HandlerId(Guid ScopeId, int LayerId, int GroupId, int Index);
}
