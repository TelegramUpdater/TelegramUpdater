using System.Collections;

namespace TelegramUpdater.Helpers;

/// <summary>
/// A grid like collection. useful for building telegram keyboards.
/// </summary>
/// <typeparam name="T">Type of element in the grid collection.</typeparam>
/// <remarks>
/// Create a new grid collection.
/// </remarks>
/// <param name="rowCapacity">
/// Specify the maximum allowed items count for a row.
/// <para>Leave <see langword="null"/> for no limit.</para>
/// </param>
public sealed class GridCollection<T>(int? rowCapacity = default) : ICollection<T>
{
    /// <summary>
    /// Returns the count of all elements in the grid.
    /// </summary>
    public int Count => Items.Sum(x => x.Count);

    /// <summary>
    /// Count the rows.
    /// </summary>
    public int RowsCount => Items.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// Get maximum item count allowed for a row.
    /// </summary>
    /// <remarks>Returns <see langword="null"/> if there's no limit.</remarks>
    public int? RowCapacity { get; } = rowCapacity;

    /// <summary>
    /// Get ready to use items of this grid.
    /// </summary>
#pragma warning disable MA0016 // Prefer using collection abstraction instead of implementation
    public List<List<T>> Items { get; } = [rowCapacity == null ? new() : new(rowCapacity.Value)];
#pragma warning restore MA0016 // Prefer using collection abstraction instead of implementation

    /// <summary>
    /// Add an item to the end of the last row of the grid.
    /// </summary>
    /// <param name="item">Item to add.</param>
    public void Add(T item)
    {
        var lastRow = Items[^1];
        // If the last row is full and autoExtend enabled, then we can add a row.
        if (RowCapacity.HasValue)
        {
            if (lastRow.Count >= RowCapacity.Value)
            {
                AddRow();
            }
        }

        Items[^1].Add(item);
    }

    /// <summary>
    /// Same as <see cref="Add(T)"/>. but returns this ( <see cref="GridCollection{T}"/> ).
    /// </summary>
    /// <param name="item">Item to add.</param>
    public GridCollection<T> AddItem(T item)
    {
        Add(item);
        return this;
    }

    /// <summary>
    /// Add a new empty row to the grid.
    /// </summary>
    public GridCollection<T> AddRow()
    {
        Items.Add(RowCapacity == null ? new() : new(RowCapacity.Value));
        return this;
    }

    /// <summary>
    /// Adds a new row and adds an item to that row.
    /// </summary>
    /// <param name="item">Item to be added.</param>
    public GridCollection<T> AddToRow(T item)
    {
        Items.Add([item]);
        return this;
    }

    /// <summary>
    /// Adds an item to the specified row.
    /// </summary>
    /// <param name="rowIndex">Index of target row.</param>
    /// <param name="item">Item to add.</param>
    public GridCollection<T> AddToRow(int rowIndex, T item)
    {
        Items[rowIndex].Add(item);
        return this;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        Items.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return Items.Exists(x => x.Exists(y => y != null && y.Equals(item)));
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotSupportedException("Grid collection doesn't support copy yet!");
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return Items.SelectMany(x => x).GetEnumerator();
    }

    /// <summary>
    /// Enumerate over rows.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IEnumerable<T>> EnumerateRows()
    {
        foreach (var l in Items)
        {
            yield return l;
        }
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        foreach (var l in Items)
        {
            if (l.Contains(item))
            {
                return l.Remove(item);
            }
        }

        return false;
    }

    /// <summary>
    /// Remove a row by index.
    /// </summary>
    /// <param name="rowIndex">Row index.</param>
    /// <returns></returns>
    public void RemoveRow(int rowIndex)
    {
        Items.RemoveAt(rowIndex);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return Items.SelectMany(x => x).GetEnumerator();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
#pragma warning disable MA0016 // Prefer using collection abstraction instead of implementation
    public static implicit operator List<List<T>>(GridCollection<T> grid) => grid.Items;
#pragma warning restore MA0016 // Prefer using collection abstraction instead of implementation
}
